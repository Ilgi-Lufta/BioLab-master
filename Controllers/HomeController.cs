using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using BioLab.Models;

namespace BioLab.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    private static Random random = new Random();


    //krijojm ne string me 8 karaktere random
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    public IActionResult homepage()
    {
        return View();
    }

    public async Task<IActionResult> Index(string searchString)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");
        if (admin == null)
        {
            return RedirectToAction("homepage");
        }

        //shfaqim listen me analiza
        var Analiz = await _context.Analizat.Where(e => e.AdminId == admin).ToListAsync();
        ViewBag.Analiz = Analiz;
        //funksioni kerkimit sipas emrit te analizes
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.Analiz = Analiz.Where(s => s.Emri!.Contains(searchString));
        }

        return View();
    }
    public async Task<IActionResult> kerkoFleta(string SearchString2)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");
        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //shfaqim listen me fleta analizash model
        var Flete = await _context.FleteAnalizes.Where(e => e.model == true).Where(e => e.AdminId == admin).ToListAsync();
        ViewBag.Flete = Flete;
        //funksioni kerkimit sipas emrit te fletes se analizes
        if (!String.IsNullOrEmpty(SearchString2))
        {
            ViewBag.Flete = Flete.Where(s => s.Emri!.Contains(SearchString2) && s.model == true);
        }

        return View();
    }

    public async Task<IActionResult> kerkoPacient(string searchString2)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //shfaqim listen me fleta analizash 
        var Flete = await _context.FleteAnalizes.Include(e => e.MyPacient).Include(e => e.mtms).ThenInclude(e => e.Myanaliz).Where(e => e.model == false).Where(e => e.AdminId == admin).ToListAsync();
        ViewBag.Flete = Flete;
        //funksioni kerkimit sipas emrit te pacientit
        if (!String.IsNullOrEmpty(searchString2))
        {
            ViewBag.Flete = Flete.Where(s => s.MyPacient.Emripacientit!.Contains(searchString2));
        }

        return View();
    }




    public IActionResult AddAnaliz()
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }


        return View();
    }

    [HttpPost]
    public IActionResult CreateAnaliz(Analiza marrngaadd)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        if (ModelState.IsValid)
        {
            //bejm kontrollin nese ekziston nje analize me kte emer e krijuar nga admini i loguar
            if ((_context.Analizat.Any(u => (u.Emri == marrngaadd.Emri) && (u.AdminId == admin))))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Emri", "Name already in use!");

                return View("AddAnaliz");
            }
            //vendosim lidhjen one to many per analizat e adminit te loguar
            // dhe e ruajm analizesn ne db
            int IntVariable = (int)HttpContext.Session.GetInt32("AdminId");
            marrngaadd.AdminId = IntVariable;
            _context.Add(marrngaadd);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("AddAnaliz");
    }


    [HttpPost]
    public IActionResult DergoMesazh(Mesazh marrngaadd)
    {
        if (ModelState.IsValid)
        {
            _context.Add(marrngaadd);
            _context.SaveChanges();
            return RedirectToAction("homepage");
        }

        return View("homepage");
    }

    public IActionResult EditAnaliz(int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        ViewBag.id = id;
        Analiza Editing = _context.Analizat.Where(e => e.AdminId == admin).FirstOrDefault(p => p.AnalizaId == id);

        return View(Editing);
    }



    public IActionResult EditFlete(int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        ViewBag.id = id;
        FleteAnalize Editing = _context.FleteAnalizes.Where(e => e.AdminId == admin).FirstOrDefault(p => p.FleteAnalizeId == id);

        return View(Editing);
    }

    public IActionResult PerdorFlete(int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //kalojm fleten e mare nga db qe id qe marim si parameter eshte e njete me fleteanalizeId
        ViewBag.id = id;
        ViewBag.thisflet = _context.FleteAnalizes.Where(e => e.AdminId == admin).FirstOrDefault(p => p.FleteAnalizeId == id);

        return View();
    }

    [HttpPost]
    public IActionResult PerdorurFlete(int id, Pacient marrngaadd)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //vendosim kryer2 ne 0 qe do te thote se
        // action Shfaqflt2  nuk eshte eskzekutuar
        HttpContext.Session.SetInt32("kryer2", 0);
        if (ModelState.IsValid)
        {
            FleteAnalize fltdb = _context.FleteAnalizes.FirstOrDefault(e => e.FleteAnalizeId == id);
            //kontrollojm nese ekziston nje pacient me nr personal e mar nga forma me i krijuar nga admini i loguar
            if (_context.Pacients.Any(e => (e.NrPersonal == marrngaadd.NrPersonal) && (e.AdminId == admin)))
            {
                //editojm moshen dhe gjinine dhe ruajm ndryshimet
                Pacient pacinetiekzistues = _context.Pacients.FirstOrDefault(e => e.NrPersonal == marrngaadd.NrPersonal);
                pacinetiekzistues.Mosha = marrngaadd.Mosha;
                pacinetiekzistues.Gjinia = marrngaadd.Gjinia;
                _context.SaveChanges();
                // krijojm nje flete analize te re me one to many te pacinetit te mar nga db
                //dhe one to many per adminin e loguar,kalojm dhe emrin e modelit te fletes se nalizes se perdorur
                FleteAnalize flrEre = new FleteAnalize
                {
                    Emri = fltdb.Emri,
                    PacientId = pacinetiekzistues.PacientId,
                    AdminId = admin
                };
                _context.Add(flrEre);
                _context.SaveChanges();
                //Marrim fleten e sapo krijuar dhe bejm redirect tek shfaqflet2 me id e fletes se sapokrijuar
                FleteAnalize fltkrijuar = _context.FleteAnalizes.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

                return RedirectToAction("Shfaqflt2", new { id = id, id2 = fltkrijuar.FleteAnalizeId });
            }
            else
            {
                //nese nga forma marim nje pacint te ri gjenerojm passwordin
                // dhe plotesojm mardhenjen one to many me adminin
                marrngaadd.Password = RandomString(8);
                marrngaadd.AdminId = admin;
                _context.Add(marrngaadd);
                _context.SaveChanges();
                //Marrim Pacinetin e sapo krijuar
                Pacient pacineti = _context.Pacients.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                // krijojm nje flete analize te re me one to many te pacinetit te mar nga db
                //dhe one to many per adminin e loguar,kalojm dhe emrin e modelit te fletes se nalizes se perdorur
                FleteAnalize flrEre = new FleteAnalize
                {
                    Emri = fltdb.Emri,
                    PacientId = pacineti.PacientId,
                    AdminId = admin
                };
                _context.Add(flrEre);
                _context.SaveChanges();
                //Marrim fleten e sapo krijuar dhe bejm redirect tek shfaqflet2 me id e fletes se sapokrijuar
                FleteAnalize fltkrijuar = _context.FleteAnalizes.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                return RedirectToAction("Shfaqflt2", new { id = id, id2 = fltkrijuar.FleteAnalizeId });
            }
        }
        else
        {
            return RedirectToAction("PerdorFlete", new { id = id });
        }
    }
     public IActionResult Shfaqflt2(string searchString, int id, int id2, float zbritja)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //marim nje liste me analiza te krijuar nga admini i loguar
        var Analiz = from m in _context.Analizat.Where(e => e.AdminId == admin)
                     select m;
        ViewBag.Analiz = Analiz;
        ///funksioni i kerkimit sipas emrit te analizes
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.Analiz = Analiz.Where(s => s.Emri!.Contains(searchString));
        }

        int kryer = (int)HttpContext.Session.GetInt32("kryer2");
        //nese ky veprim nuk eshte kryer asnjehere dmth kryer = 0 
        if (kryer == 0)
        {
            //kalojme totalin nga fleta analizes model tek fleta e analizes qe sapo krijuam te re(ilgi2)
            FleteAnalize ilgi = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id);
            FleteAnalize ilgi2 = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
            ilgi2.Totali = ilgi.Totali;
            ilgi2.Paguar = ilgi2.Totali;
            _context.SaveChanges();
//kalojme tek fleta e re e e anlizes mardhenjet many to many 
//te fletes se anlizes qe meret si model
            foreach (var item in ilgi.mtms)
            {
                mtm mymtm = new mtm()
                {
                    AnalizaId = item.AnalizaId,
                    FleteAnalizeId = id2
                };
                _context.Add(mymtm);
                _context.SaveChanges();
            }
        }
        //nese parametri zbritja eshte i ndryshem nga 0 kryej funksionin e meposhtem
        FleteAnalize ilgi3 = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        if (zbritja != 0)
        {
            ilgi3.Paguar = ilgi3.Totali - zbritja;
            ilgi3.Zbritja = zbritja;
            _context.SaveChanges();
        }
        //vendosim kryer2 ne vleren 1 qe do te thote se
        // ky funksion eshte ekzekutuar nje here
        HttpContext.Session.SetInt32("kryer2", 1);
        ViewBag.thisflet = _context.FleteAnalizes.Include(e => e.MyPacient).Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        ViewBag.idmodel = id;
        return View();
    }

    public IActionResult LogIn()
    {
        return View();
    }

    public IActionResult LogInAdmin()
    {

        return View();
    }

    [HttpPost]
    public IActionResult LogIn(LoginUser user)
    {
        if (ModelState.IsValid)
        {

            var userInDb = _context.Pacients.FirstOrDefault(u => u.NrPersonal == user.NrPersonal);
            //kontrollojme nese usseInDb ekzsiton
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("NrPersonal", "Invalid NrPersonal/Password");
                return View("LogIn");
            }
            // kontrollojm nese passwordi eshte i sakte
            if (user.Password == userInDb.Password)
            {
                HttpContext.Session.SetInt32("UserId", userInDb.PacientId);
                return RedirectToAction("MyTestResult");
            }


        }
        return View("LogIn");
    }

    [HttpPost]
    public IActionResult LogInAdmin(LoginUser user)
    {
        if (ModelState.IsValid)
        {
            var userInDb = _context.Admins.FirstOrDefault(u => u.Username == user.NrPersonal);
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Username", "Invalid Username/Password");
                return View("LogInAdmin");
            }

            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();

            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);


            // result can be compared to 0 for failure
            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                // handle failure (this should be similar to how "existing email" is handled)
                return View("LogInAdmin");
            }
            //vendosim sesion si id e adminit ne db
            HttpContext.Session.SetInt32("AdminId", userInDb.AdminId);
            return RedirectToAction("Home");
        }
        return View("LogInAdmin");
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.SetInt32("UserId", 0);

        return RedirectToAction("homepage");
    }
    public IActionResult LogOutAdmin()
    {
        HttpContext.Session.SetInt32("AdminId", 0);

        return RedirectToAction("homepage");
    }

    [HttpPost]
    public IActionResult EditedAnaliz(int id, Analiza marrngaadd)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        if (ModelState.IsValid)
        {
            //bejm kontrollin nese ekziston nje analize me kte emer e krijuar nga admini i loguar
            if ((_context.Analizat.Any(u => (u.Emri == marrngaadd.Emri) && (u.AdminId == admin))))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Emri", "Name already in use!");

                return RedirectToAction("EditAnaliz", new { id = id });
            }
            //marrim nga db anzlizen qe duam te bejm edit dhe vendosim vlerat qe marim nga forma
            Analiza editing = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id);
            editing.Emri = marrngaadd.Emri;
            editing.Njesia = marrngaadd.Njesia;
            editing.Norma = marrngaadd.Norma;
            editing.Cmimi = marrngaadd.Cmimi;
            editing.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return RedirectToAction("EditAnaliz", new { id = id });
    }

    [HttpPost]
    public IActionResult EditedFlete(int id, FleteAnalize marrngaadd)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        if (ModelState.IsValid)
        {
            //bejm kontrollin nese ekziston nje flete analize me kte emer e krijuar nga admini i loguar
            if (_context.FleteAnalizes.Any(u => (u.Emri == marrngaadd.Emri) && (u.AdminId == admin)))
            {
                ModelState.AddModelError("Emri", "Fleta Analizes ekziston!");
                return RedirectToAction("EditFlete", new { id = id });
            }
            else
            {
                //marrim nga db anzlizen qe duam te bejm edit dhe vendosim vlerat qe marim nga forma
                FleteAnalize editing = _context.FleteAnalizes.FirstOrDefault(p => p.FleteAnalizeId == id); // marrengaadd.DishId nuk mund te zevendesohet me id sepse nxjerr problem
                editing.Emri = marrngaadd.Emri;
                editing.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction("Shfaqflt", new { id = id });
            }
        }
        return RedirectToAction("EditFlete", new { id = id });

    }


    public IActionResult FshiAnaliz(int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //fshijme analizen e marre nga db me analizId si parametri id
        Analiza removingAnaliza = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id);
        _context.Analizat.Remove(removingAnaliza);
        _context.SaveChanges();
        return RedirectToAction("Index");

    }

    public IActionResult FshiFlete(int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");
        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //fshijme fleten e analizes e marre nga db me analizId si parametri id
        FleteAnalize removingAnaliza = _context.FleteAnalizes.FirstOrDefault(p => p.FleteAnalizeId == id);
        _context.FleteAnalizes.Remove(removingAnaliza);
        _context.SaveChanges();
        return RedirectToAction("kerkoFleta");
    }

    public IActionResult AddFleteAnalize()

    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }

        return View();
    }

    [HttpPost]
    public IActionResult CreateFletAnalize(FleteAnalize marrngaadd)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }

        if (ModelState.IsValid)
        {
            //bejm kontrollin nese ekziston nje flete analize me kte emer e krijuar nga admini i loguar
            if ((_context.FleteAnalizes.Any(u => (u.Emri == marrngaadd.Emri) && (u.AdminId == admin))))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Emri", "Name already in use!");

                // You may consider returning to the View at this point
                return View("AddFleteAnalize");
            }
            //krijojme nje pacient model per kte flete analize model
            Pacient ilgi = new Pacient()
            {
                Emripacientit = "Model",
                Gjinia = "Model",
                Tipi = "Model",
                Mosha = 99,
                NrPersonal = "Model",
                AdminId = admin
            };
            _context.Add(ilgi);
            _context.SaveChanges();
            //krijojme mardhenjet one to many te fletes se analizes me adminin dhe pacientin e sapokrijuar
            Pacient Pdb = _context.Pacients.OrderByDescending(e => e.CreatedAt).FirstOrDefault();

            marrngaadd.model = true;
            marrngaadd.PacientId = Pdb.PacientId;
            marrngaadd.AdminId = admin;

            _context.Add(marrngaadd);
            _context.SaveChanges();
            //kalojme si parameter fleteanalizeid te fletes te sapokrijuar dhe bejm rederect
            FleteAnalize fltdb = _context.FleteAnalizes.OrderByDescending(e => e.CreatedAt).FirstOrDefault();

            return RedirectToAction("Shfaqflt", new { id = fltdb.FleteAnalizeId });
        }
        return View("AddFleteAnalize");
    }

    public IActionResult Shfaqflt(string searchString, int id)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //kalojm listen me analiza
        var Analiz = from m in _context.Analizat.Where(e => e.AdminId == admin)
                     select m;
        ViewBag.Analiz = Analiz;
        //bejme kerkimin sipas emrit te analizes
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.Analiz = Analiz.Where(s => s.Emri!.Contains(searchString));
        }

        ViewBag.thisflet = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).Where(e => e.AdminId == admin).FirstOrDefault(e => e.FleteAnalizeId == id);

        return View();
    }

    public IActionResult SHtoneliste(int id, int id2)
    {
        //nese ekzstion many to many midis analizes dhe fletes se analizes
        // mos te krijohet 2 here ne db kjo many to many
        List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
        if (allmtm.Contains(dbmtm))
        {
            return RedirectToAction("Shfaqflt", new { id = id2 });
        }
        else
        {
            //nese many to many nuk ekziston krijom nje te re me parametrat e mar nhga funksioni
            mtm ilgi = new mtm()
            {
                AnalizaId = id,
                FleteAnalizeId = id2
            };
            _context.Add(ilgi);
            //per cdo analiz qe i shtohet fletes se analizes shtojm cmimin tnga totali
            Analiza an1 = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);
            FleteAnalize flt1 = _context.FleteAnalizes.FirstOrDefault(e => e.FleteAnalizeId == id2);
            flt1.Totali = an1.Cmimi + flt1.Totali;
            flt1.Paguar = flt1.Totali;
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt", new { id = id2 });
        }
    }
    //shfaq fleten e  analizes gati per printim
    public IActionResult Printo(int id2)
    {
        ViewBag.thisflet = _context.FleteAnalizes.Include(e => e.MyPacient).Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        return View();
    }

    public IActionResult Hiqngalista(int id, int id2)
    {
        //nese ekzstion many to many midis analizes dhe fletes se analizes i bejm remove nga db
        List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
        if (allmtm.Contains(dbmtm))
        {
            //per cdo analiz qe i hiqet fletes se analizes zbresim cmimin nga totali
            Analiza an1 = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);
            FleteAnalize flt1 = _context.FleteAnalizes.FirstOrDefault(e => e.FleteAnalizeId == id2);

            flt1.Totali = flt1.Totali - an1.Cmimi;
            flt1.Paguar = flt1.Totali;

            _context.mtms.Remove(dbmtm);
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt", new { id = id2 });
        }
        else
        {
            return RedirectToAction("Shfaqflt", new { id = id2 });
        }
    }

    [HttpPost]
    public IActionResult Save(int id, int id2, float vlera)
    {
        //Ruajm vleren e mar nga forma tek analiza e mar nga db
        Analiza dbanaliz = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);

        dbanaliz.Rezultati = vlera;
        dbanaliz.UpdatedAt = DateTime.Now;
        _context.SaveChanges();

        return RedirectToAction("Shfaqflt", new { id = id2 });
    }

   

    public IActionResult SHtoneliste2(int id, int id2, int id3)
    {
        //nese ekzstion many to many midis analizes dhe fletes se analizes
        // mos te krijohet 2 here ne db kjo many to many
        List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
        if (allmtm.Contains(dbmtm))
        {
            return RedirectToAction("Shfaqflt2", new { id2 = id2, id = id3 });
        }
        else
        {
            //nese many to many nuk ekziston krijom nje te re me parametrat e mar nhga funksioni
            mtm ilgi = new mtm()
            {
                AnalizaId = id,
                FleteAnalizeId = id2
            };
            _context.Add(ilgi);
            //per cdo analiz qe i shtohet fletes se analizes shtojm cmimin tnga totali
            Analiza an1 = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);
            FleteAnalize flt1 = _context.FleteAnalizes.FirstOrDefault(e => e.FleteAnalizeId == id2);
            flt1.Totali = an1.Cmimi + flt1.Totali;
            flt1.Paguar = flt1.Totali;
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt2", new { id2 = id2, id = id3 });
        }
    }

    //shfaq fleten e  analizes gati per printim
    public IActionResult Printo2(int id2)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        ViewBag.thisflet = _context.FleteAnalizes.Include(e => e.MyPacient).Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        return View();
    }

    //shfaq fleten e  analizes gati per printim
    public IActionResult Printo3(int id2)
    {
        ViewBag.thisflet = _context.FleteAnalizes.Include(e => e.MyPacient).Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        return View();
    }


    public IActionResult Hiqngalista2(int id, int id2, int id3)
    {
        //nese ekzstion many to many midis analizes dhe fletes se analizes i bejm remove nga db
        List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
        if (allmtm.Contains(dbmtm))
        {
            //per cdo analiz qe i hiqet fletes se analizes zbresim cmimin nga totali
            Analiza an1 = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);
            FleteAnalize flt1 = _context.FleteAnalizes.FirstOrDefault(e => e.FleteAnalizeId == id2);
            flt1.Totali = flt1.Totali - an1.Cmimi;
            flt1.Paguar = flt1.Totali;

            _context.mtms.Remove(dbmtm);
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt2", new { id2 = id2, id = id3 });
        }
        else
        {
            return RedirectToAction("Shfaqflt2", new { id2 = id2, id = id3 });
        }
    }

    //Ruajm vleren e mar nga forma tek analiza e mar nga db
    [HttpPost]
    public IActionResult Save2(int id, int id2, float vlera, int idmodel)
    {

        Analiza dbanaliz = _context.Analizat.FirstOrDefault(e => e.AnalizaId == id);

        dbanaliz.Rezultati = vlera;
        dbanaliz.UpdatedAt = DateTime.Now;
        _context.SaveChanges();

        return RedirectToAction("Shfaqflt2", new { id2 = id2, id = idmodel });
    }
    //vendosim atributin pagesa ne true te fletes se analises se marr nga db
    public IActionResult Paguar(int id, int id2)
    {
        FleteAnalize ilgi2 = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id2);
        ilgi2.Pagesa = true;

        _context.SaveChanges();

        return RedirectToAction("Shfaqflt2", new { id2 = id2, id = id });
    }
    //vendosim atributin pagesa ne true te fletes se analises se marr nga db
    public IActionResult Paguar3(int id)
    {
        FleteAnalize ilgi2 = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id);
        ilgi2.Pagesa = true;

        _context.SaveChanges();

        return RedirectToAction("Shfaqflt", new { id = id });
    }
    //vendosim atributin pagesa ne true te fletes se analises se marr nga db

    public IActionResult Paguar2(int id)
    {
        FleteAnalize ilgi2 = _context.FleteAnalizes.Include(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.FleteAnalizeId == id);
        ilgi2.Pagesa = true;

        _context.SaveChanges();
        return RedirectToAction("printo3", new { id2 = id });
    }

    public async Task<IActionResult> KerkoMeDate(DateTime searchFirstTime, DateTime searchSecondTime)
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        //funksioni kerkimit sipas Dates te fletes se analizes
        List<FleteAnalize> ilgi = await _context.FleteAnalizes.Include(e => e.MyPacient).Where(e => e.AdminId == admin).Where(s => s.CreatedAt > searchFirstTime && s.CreatedAt < searchSecondTime/* && s.Pagesa == true*/).Where(e => e.model == false).ToListAsync();
        ViewBag.Analiz = ilgi;
        float Totali = 0;
        //ruajm ne total shumen e faturave te paguara
        foreach (var item in ilgi)
        {
            if (item.Pagesa == true)
            {
                Totali = Totali + item.Paguar;
            }

        }
        ViewBag.totali = Totali;
        return View();
    }
    //shfaqim listen me analiza nga pacienti i loguar
    public IActionResult MyTestResult()
    {
        int? id = (int)HttpContext.Session.GetInt32("UserId");
        if (id != null)
        {
            ViewBag.thispacient = _context.Pacients.Include(e => e.MYfleteanaliz).ThenInclude(e => e.mtms).ThenInclude(e => e.Myanaliz).FirstOrDefault(e => e.PacientId == id);
            return View();
        }
        else
        {
            return RedirectToAction("homepage");
        }

    }

    public IActionResult Home()
    {
        int admin = (int)HttpContext.Session.GetInt32("AdminId");

        if (admin == null)
        {
            return RedirectToAction("homepage");
        }
        ViewBag.Admini = _context.Admins.FirstOrDefault(e => e.AdminId == admin);

        var lastmonth = DateTime.Today.AddMonths(-1);
        var towmonthsAgo = DateTime.Today.AddMonths(-2);

        var lastyear = DateTime.Today.AddMonths(-12);
        var towyearsAgo = DateTime.Today.AddMonths(-24);
        //pacinentet ne total
        ViewBag.pacientet = _context.Pacients.Where(e => e.AdminId == admin).Where(e => e.Emripacientit != "Model").Count();
        //pacintet qe nuk kane ber pagesen
        ViewBag.pacientetpapaguar = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.Pagesa == false).Count();
        //pacientet vitn dhe muajin e fundit
        ViewBag.pacientetpernjevit = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastyear).Count();
        ViewBag.pacientetpernjemuaj = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastmonth).Count();
        //xhiro e bere per vitn dhe muajin e fundit
        ViewBag.revenewpernjevit = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastyear).Sum(e => e.Paguar);
        ViewBag.revenewmuajinefundit = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastmonth).Sum(e => e.Paguar);
        //lista me mesazhe
        ViewBag.Mesazh = _context.Mesazhs.Where(e => e.Lexuar == false).ToList();
        //zbresim numrin e pacinteve nga numrin e testeve te kryera 
        var Analiza = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Count();
        var Pacinte = _context.Pacients.Where(e => e.AdminId == admin).Where(e => e.Emripacientit != "Model").Count();
        ViewBag.erdhenperseri = Analiza - Pacinte;

        var lastyearicome = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastyear).Sum(e => e.Paguar);
        var twoyearsAgoicome = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => (e.CreatedAt < lastyear) && (e.CreatedAt > towyearsAgo)).Sum(e => e.Paguar);
        ViewBag.Revenewcomparisom2yearsAgoVSlastyear =lastyearicome- twoyearsAgoicome;

        var lastmonthincome = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => e.CreatedAt > lastmonth).Sum(e => e.Paguar);
        var twomonthsincome = _context.FleteAnalizes.Where(e => e.AdminId == admin).Where(e => e.model == false).Where(e => (e.CreatedAt < lastmonth) && (e.CreatedAt > towmonthsAgo)).Sum(e => e.Paguar);
        ViewBag.Revenewcomparisom2monthssAgoVSlastmonth =lastmonthincome-twomonthsincome;


        return View();
    }
    //vendosim atrubutin lexuar te mesazhit ne true
    public IActionResult Lexuar(int id)
    {
        Mesazh mesazhdb = _context.Mesazhs.FirstOrDefault(e => e.MesazhID == id);
        mesazhdb.Lexuar = true;
        _context.SaveChanges();

        return RedirectToAction("Home");
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(Admin user)
    {
        if (ModelState.IsValid)
        {
            //kotrollojme nese ekziston nje admin nje db me kte username
            if (_context.Admins.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username already in use!");
                return View("Register");
            }
            //krijojm nje objekt passwordhasher<admin> 
            //dhe ruajm paswordin te pasi i bjem hash
            PasswordHasher<Admin> Hasher = new PasswordHasher<Admin>();
            user.Password = Hasher.HashPassword(user, user.Password);

            _context.Admins.Add(user);
            _context.SaveChanges();
            Admin Userdb = _context.Admins.FirstOrDefault(u => u.Username == user.Username);

            HttpContext.Session.SetInt32("AdminId", Userdb.AdminId);
            int IntVariable = (int)HttpContext.Session.GetInt32("AdminId");

            return RedirectToAction("Home");
        }
        else
        {
            return View("Register");
        }

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
