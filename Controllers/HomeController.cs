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

public static string RandomString(int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}



    public async Task<IActionResult> Index(string searchString)
{
    var  Analiz =  from m in _context.Analizat
                 select m;
    ViewBag.Analiz =  Analiz;
    
    if (!String.IsNullOrEmpty(searchString))
    {
       ViewBag.Analiz  = Analiz.Where(s => s.Emri!.Contains(searchString));
    }

    return View();
}
    public async Task<IActionResult> kerkoFleta(string SearchString2)
{
   var Flete = _context.FleteAnalizes.Where(e=>e.model==true).ToList();
    ViewBag.Flete =  Flete;
    
    if (!String.IsNullOrEmpty(SearchString2))
    {
       ViewBag.Flete  = Flete.Where(s => s.Emri!.Contains(SearchString2) && s.model==true);
    }

    return View();
}

    public async Task<IActionResult> kerkoPacient(string searchString2)
{
   
    var Flete = _context.FleteAnalizes.Include(e=>e.MyPacient).Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).Where(e=>e.model==false).ToList();
    ViewBag.Flete =  Flete;
    
    if (!String.IsNullOrEmpty(searchString2))
    {
       ViewBag.Flete  = Flete.Where(s => s.MyPacient.Emripacientit!.Contains(searchString2));
    }

    return View();
}

 

    public IActionResult AddAnaliz()
    {
        
        return View();
    }

    [HttpPost]
    public IActionResult CreateAnaliz(Analiza marrngaadd)
    {
        if (ModelState.IsValid)
        {
            if (_context.Analizat.Any(u => u.Emri == marrngaadd.Emri))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Emri", "Kjo analize ekziston!");

                // You may consider returning to the View at this point
                return View("AddAnaliz");
            }


                // int IntVariable = (int)HttpContext.Session.GetInt32("UserId");
                // marrngaadd.UserId = IntVariable;
                
                _context.Add(marrngaadd);
                _context.SaveChanges(); 
                return RedirectToAction("Index");
            }


        
        return View("AddAnaliz");
    }

     public IActionResult EditAnaliz(int id)
    {
        
        ViewBag.id=id;
        Analiza Editing = _context.Analizat.FirstOrDefault(p=>p.AnalizaId == id);
        
        return View(Editing);
    }

    

      public IActionResult EditFlete(int id)
    {
        
        ViewBag.id=id;
        FleteAnalize Editing = _context.FleteAnalizes.FirstOrDefault(p=>p.FleteAnalizeId == id);
        
        return View(Editing);
    }

      public IActionResult PerdorFlete(int id)
    {
        
        ViewBag.id=id;
        ViewBag.thisflet = _context.FleteAnalizes.FirstOrDefault(p=>p.FleteAnalizeId == id);
        
        return View();
    }

    

     [HttpPost]
    public IActionResult PerdorurFlete(int id,Pacient marrngaadd)
    {
          HttpContext.Session.SetInt32("kryer2",0);
        if (ModelState.IsValid)
        {
            FleteAnalize fltdb = _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId == id); 

           if(_context.Pacients.Any(e=>e.Emripacientit == marrngaadd.Emripacientit)){
            Pacient pacinetiekzistues = _context.Pacients.FirstOrDefault(e=>e.Emripacientit == marrngaadd.Emripacientit);

            FleteAnalize flrEre = new FleteAnalize{
                Emri = fltdb.Emri,
                PacientId = pacinetiekzistues.PacientId
            };
            _context.Add(flrEre);
            _context.SaveChanges();

            FleteAnalize fltkrijuar = _context.FleteAnalizes.OrderByDescending(p=>p.CreatedAt).FirstOrDefault();

            return RedirectToAction("Shfaqflt2", new{id =id,id2=fltkrijuar.FleteAnalizeId});
           }


           else{
          
 marrngaadd.Password = RandomString(8);

//  PasswordHasher<Pacient> Hasher = new PasswordHasher<Pacient>();
//              marrngaadd.Password = Hasher.HashPassword(marrngaadd, marrngaadd.Password);


            _context.Add(marrngaadd);
            _context.SaveChanges();

             Pacient pacineti = _context.Pacients.OrderByDescending(p=>p.CreatedAt).FirstOrDefault();
              FleteAnalize flrEre = new FleteAnalize{
                Emri = fltdb.Emri,
                PacientId = pacineti.PacientId
            };
             _context.Add(flrEre);
            _context.SaveChanges();

          
           
 FleteAnalize fltkrijuar = _context.FleteAnalizes.OrderByDescending(p=>p.CreatedAt).FirstOrDefault();
  return RedirectToAction("Shfaqflt2", new{id =id,id2=fltkrijuar.FleteAnalizeId});
           }
            
        }
        else
        {
            
            // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
             return RedirectToAction("PerdorFlete",new { id = id });
            //   return RedirectToAction("Edit",new { id = id}); // return view nuk punon kur ben submit 2 here nje fushe te gabuar
        }
    }

    public IActionResult LogIn(){

        return View();
    }

[HttpPost]
    public IActionResult LogIn(LoginUser user)
    {
        if (ModelState.IsValid)
        {
            // If initial ModelState is valid, query for a user with provided email
            var userInDb = _context.Pacients.FirstOrDefault(u => u.Emripacientit == user.Emripacientit);
            // If no user exists with provided email
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Username", "Invalid Email/Password");
                return View("LogIn");
            }

            // Initialize hasher object
            // var hasher = new PasswordHasher<LoginUser>();

            // // verify provided password against hash stored in db
            // var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);


            // // result can be compared to 0 for failure
            // if (result == 0)
            // {
            //     ModelState.AddModelError("Password", "Invalid Password");
            //     // handle failure (this should be similar to how "existing email" is handled)
            //     return View("LogIn");
            // }
             
            if(userInDb.Password != user.Password){
                ModelState.AddModelError("Password", "Invalid Password");
                return View("LogIn");
            }

            HttpContext.Session.SetInt32("UserId", userInDb.PacientId);
            return RedirectToAction("MyTestResult");
        }
        return View("LogIn");}

//T78AXNHS
    [HttpPost]
    public IActionResult EditedAnaliz(int id,Analiza marrngaadd)
    {
        if (ModelState.IsValid)
        {
          

            Analiza editing = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id); // marrengaadd.DishId nuk mund te zevendesohet me id sepse nxjerr problem
           editing.Emri = marrngaadd.Emri;
           editing .Njesia = marrngaadd.Njesia;
           editing .Norma = marrngaadd.Norma;
           editing .Cmimi = marrngaadd.Cmimi;
           editing.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
            }
        
        else
        {
             Analiza editing = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id);
            // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
            return RedirectToAction("Index");
            //   return RedirectToAction("Edit",new { id = id}); // return view nuk punon kur ben submit 2 here nje fushe te gabuar
        }
    }

    [HttpPost]
    public IActionResult EditedFlete(int id,FleteAnalize marrngaadd)
    {
        if (ModelState.IsValid)
        {
            if (_context.FleteAnalizes.Any(u => u.Emri == marrngaadd.Emri))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                FleteAnalize Editing1 = _context.FleteAnalizes.FirstOrDefault(p => p.FleteAnalizeId==id);
                ModelState.AddModelError("Emri", "Fleta Analizes ekziston!");
                // You may consider returning to the View at this point
                return RedirectToAction("EditFlete",new { id = id });
            }else{

           FleteAnalize  editing = _context.FleteAnalizes.FirstOrDefault(p => p.FleteAnalizeId == id); // marrengaadd.DishId nuk mund te zevendesohet me id sepse nxjerr problem
           editing.Emri = marrngaadd.Emri;
  
           editing.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt", new{id =id

                });
            }
        }
        else
        {
             Analiza editing = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id);
            // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
             return RedirectToAction("EditFlete",new { id = id });
            //   return RedirectToAction("Edit",new { id = id}); // return view nuk punon kur ben submit 2 here nje fushe te gabuar
        }
    }


     public IActionResult FshiAnaliz(int id)
    {
        Analiza removingAnaliza = _context.Analizat.FirstOrDefault(p => p.AnalizaId == id);

        _context.Analizat.Remove(removingAnaliza);
        _context.SaveChanges();
        // return View("Index");
        return RedirectToAction("Index"); // pasi bejm return view  ben id=0 dhe nxjerr prb

    }



 public IActionResult FshiFlete(int id)
    {
        FleteAnalize removingAnaliza = _context.FleteAnalizes.FirstOrDefault(p => p.FleteAnalizeId == id);

        _context.FleteAnalizes.Remove(removingAnaliza);
        _context.SaveChanges();
        // return View("Index");
        return RedirectToAction("kerkoFleta"); // pasi bejm return view  ben id=0 dhe nxjerr prb

    }
 
    public async Task<IActionResult> AddFleteAnalize(string searchString)

    {
        var Analiz = from m in _context.Analizat
                 select m;
    ViewBag.Analiz =  Analiz;
    
    if (!String.IsNullOrEmpty(searchString))
    {
       ViewBag.Analiz  = Analiz.Where(s => s.Emri!.Contains(searchString));
    }
        return View();
    }

     [HttpPost]
    public IActionResult CreateFletAnalize(FleteAnalize marrngaadd)
    {
      

        if (ModelState.IsValid)
        {  Pacient ilgi = new Pacient(){
            Emripacientit = "Model",
            Gjinia= "Model",
            Tipi = "Model",
            Mosha = 99
        };
         _context.Add(ilgi);
                _context.SaveChanges(); 
                Pacient Pdb = _context.Pacients.OrderByDescending(e=>e.CreatedAt).FirstOrDefault();



                marrngaadd.model=true;
                marrngaadd.PacientId= Pdb.PacientId;

                
                _context.Add(marrngaadd);
                _context.SaveChanges(); 
                FleteAnalize fltdb = _context.FleteAnalizes.OrderByDescending(e=>e.CreatedAt).FirstOrDefault();
               
                return RedirectToAction("Shfaqflt", new{id =fltdb.FleteAnalizeId

                });
            }


        
        return View("AddFleteAnalize");
    }

      public IActionResult Shfaqflt(string searchString,int id)
    {
      
     var Analiz = from m in _context.Analizat
                 select m;
    ViewBag.Analiz =  Analiz;
    
    if (!String.IsNullOrEmpty(searchString))
    {
       ViewBag.Analiz  = Analiz.Where(s => s.Emri!.Contains(searchString));
    }

ViewBag.thisflet= _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);
    
        return View();
    }
          public IActionResult Shfaqflt3(string searchString,int id,float zbritja)
    {
      
     var Analiz = from m in _context.Analizat
                 select m;
    ViewBag.Analiz =  Analiz;
    
    if (!String.IsNullOrEmpty(searchString))
    {
       ViewBag.Analiz  = Analiz.Where(s => s.Emri!.Contains(searchString));
    }
    FleteAnalize  ilgi3  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);

if(zbritja!=0){
    ilgi3.Paguar = ilgi3.Totali - zbritja;
    ilgi3.Zbritja = zbritja;
        _context.SaveChanges();
}

ViewBag.thisflet= _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);
    
        return View();
    }
    
    


 public IActionResult SHtoneliste(int id,int id2)
    {

  List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            return RedirectToAction("Shfaqflt", new{id = id2

                });

        }else{
            mtm ilgi =  new mtm(){
            AnalizaId=id,
            FleteAnalizeId=id2
        };
         _context.Add(ilgi);
        Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = an1.Cmimi+flt1.Totali;
        flt1.Paguar = flt1.Totali;
            _context.SaveChanges();
  return RedirectToAction("Shfaqflt", new{id = id2

                });
        }
    }

    public IActionResult SHtoneliste3(int id,int id2)
    {

  List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            return RedirectToAction("Shfaqflt3", new{id = id2

                });

        }else{
            mtm ilgi =  new mtm(){
            AnalizaId=id,
            FleteAnalizeId=id2
        };
         _context.Add(ilgi);
        Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = an1.Cmimi+flt1.Totali;
        flt1.Paguar = flt1.Totali;
            _context.SaveChanges();
  return RedirectToAction("Shfaqflt3", new{id = id2

                });
        }
    }

 public IActionResult Printo(int id2)
    {
    ViewBag.thisflet = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
     return View();
    }


    public IActionResult Hiqngalista(int id,int id2)
    {
    List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = flt1.Totali-an1.Cmimi;
        flt1.Paguar = flt1.Totali;
          
            _context.mtms.Remove(dbmtm);
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt", new{id = id2

                });
        }
        else
        {

           return RedirectToAction("Shfaqflt", new{id = id2

                });
        }
    }
    public IActionResult Hiqngalista3(int id,int id2)
    {
    List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = flt1.Totali-an1.Cmimi;
        flt1.Paguar = flt1.Totali;
          
            _context.mtms.Remove(dbmtm);
            _context.SaveChanges();
            return RedirectToAction("Shfaqflt3", new{id = id2

                });
        }
        else
        {
           return RedirectToAction("Shfaqflt3", new{id = id2

                });
        }
    }
    [HttpPost]
 public IActionResult Save(int id,int id2,float vlera){

Analiza dbanaliz = _context.Analizat.FirstOrDefault(e=>e.AnalizaId==id);

dbanaliz.Rezultati = vlera;

           dbanaliz.UpdatedAt = DateTime.Now;
            _context.SaveChanges();


    return RedirectToAction("Shfaqflt", new{id = id2

                });
 }


     public IActionResult Shfaqflt2(string searchString,int id,int id2,float zbritja)
    {
      
     var Analiz = from m in _context.Analizat
                 select m;
    ViewBag.Analiz =  Analiz;
    
    if (!String.IsNullOrEmpty(searchString))
    {
       ViewBag.Analiz  = Analiz.Where(s => s.Emri!.Contains(searchString));
    }
 
            int kryer = (int)HttpContext.Session.GetInt32("kryer2");

if(kryer==0){


FleteAnalize  ilgi  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);
FleteAnalize  ilgi2  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
ilgi2.Totali=ilgi.Totali;
ilgi2.Paguar = ilgi2.Totali;
 
 _context.SaveChanges();

  
foreach (var item in ilgi.mtms)
{ 

    mtm mymtm =  new mtm(){
            AnalizaId=item.AnalizaId,
            FleteAnalizeId=id2
        };
        _context.Add(mymtm);
        _context.SaveChanges();
}


}
FleteAnalize  ilgi3  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
if(zbritja!=0){
    ilgi3.Paguar = ilgi3.Totali - zbritja;
    ilgi3.Zbritja = zbritja;
        _context.SaveChanges();
}

           HttpContext.Session.SetInt32("kryer2",1); 
ViewBag.thisflet = _context.FleteAnalizes.Include(e=>e.MyPacient).Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);

    ViewBag.idmodel = id;
        return View();
    }

 public IActionResult SHtoneliste2(int id,int id2,int id3)
    {

  List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            //  HttpContext.Session.SetInt32("kryer2",1); 
            return RedirectToAction("Shfaqflt2", new{id2 = id2,id=id3

                });

        }else{
            mtm ilgi =  new mtm(){
            AnalizaId=id,
            FleteAnalizeId=id2
        };
         _context.Add(ilgi);
        Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = an1.Cmimi+flt1.Totali;
        flt1.Paguar = flt1.Totali;
            _context.SaveChanges();
            // HttpContext.Session.SetInt32("kryer2",1); 
  return RedirectToAction("Shfaqflt2", new{id2 = id2,id=id3

                });
        }
    }

 public IActionResult Printo2(int id2)
    {
       
    ViewBag.thisflet = _context.FleteAnalizes.Include(e=>e.MyPacient).Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
     return View();
    }
     public IActionResult Printo3(int id2)
    {
    ViewBag.thisflet =  _context.FleteAnalizes.Include(e=>e.MyPacient).Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
     return View();
    }


    public IActionResult Hiqngalista2(int id,int id2,int id3)
    {
    List<mtm> allmtm = _context.mtms.ToList();
        mtm dbmtm = _context.mtms.FirstOrDefault(p => p.AnalizaId == id && p.FleteAnalizeId == id2);
         if (allmtm.Contains(dbmtm))
        {
            Analiza an1= _context.Analizat.FirstOrDefault(e=>e.AnalizaId == id);
        FleteAnalize flt1= _context.FleteAnalizes.FirstOrDefault(e=>e.FleteAnalizeId ==id2);
        flt1.Totali = flt1.Totali-an1.Cmimi;
        flt1.Paguar = flt1.Totali;
          
            _context.mtms.Remove(dbmtm);
            _context.SaveChanges();
            // HttpContext.Session.SetInt32("kryer2",1); 
            return RedirectToAction("Shfaqflt2", new{id2 = id2,id=id3

                });
        }
        else
        {
            // HttpContext.Session.SetInt32("kryer2",1); 
           return RedirectToAction("Shfaqflt2", new{id2 = id2,id=id3

                });
        }
    }
    [HttpPost]
 public IActionResult Save2(int id,int id2,float vlera,int idmodel){

Analiza dbanaliz = _context.Analizat.FirstOrDefault(e=>e.AnalizaId==id);

dbanaliz.Rezultati = vlera;

           dbanaliz.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

    // HttpContext.Session.SetInt32("kryer2",1); 
    return RedirectToAction("Shfaqflt2", new{id2 = id2,id=idmodel

                });
 }
    
public IActionResult Paguar(int id,int id2)
    {
        FleteAnalize  ilgi2  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id2);
        ilgi2.Pagesa=true;

        _context.SaveChanges();

            return RedirectToAction("Shfaqflt2", new{id2 = id2,id=id

                });
    }
    public IActionResult Paguar3(int id)
    {
        FleteAnalize  ilgi2  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);
        ilgi2.Pagesa=true;

        _context.SaveChanges();

           return RedirectToAction("Shfaqflt", new{id =id});
    }

    public IActionResult Paguar2(int id)
    {
        FleteAnalize  ilgi2  = _context.FleteAnalizes.Include(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.FleteAnalizeId == id);
        ilgi2.Pagesa=true;

        _context.SaveChanges();

            return RedirectToAction("printo3", new{id2=id

                });
    }

        public async Task<IActionResult> KerkoMeDate(DateTime searchFirstTime,DateTime searchSecondTime)
{
       List<FleteAnalize> ilgi  = _context.FleteAnalizes.Include(e=>e.MyPacient).Where(s => s.CreatedAt>searchFirstTime && s.CreatedAt<searchSecondTime/* && s.Pagesa == true*/).Where(e=>e.model==false).ToList();
    ViewBag.Analiz = ilgi;
    //    ViewBag.Analiz  = _context.FleteAnalizes.Where(s => s.CreatedAt>searchFirstTime && s.CreatedAt<searchSecondTime);
       float Totali = 0;
       foreach (var item in ilgi)
       {
        Totali = Totali + item.Paguar;
       }
    ViewBag.totali = Totali;
    return View();
}

   public IActionResult MyTestResult()
    { int id = (int)HttpContext.Session.GetInt32("UserId");

        ViewBag.thispacient = _context.Pacients.Include(e=>e.MYfleteanaliz).ThenInclude(e=>e.mtms).ThenInclude(e=>e.Myanaliz).FirstOrDefault(e=>e.PacientId == id);
       return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
