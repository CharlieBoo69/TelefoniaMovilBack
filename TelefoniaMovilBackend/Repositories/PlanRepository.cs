//using TelefoniaMovilBackend.Data;
//using TelefoniaMovilBackend.Models;
//using System.Collections.Generic;
//using System.Linq;

//public class PlanRepository : IPlanRepository
//{
//    private readonly ApplicationDbContext _context;

//    public PlanRepository(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public IEnumerable<Plan> GetAll()
//    {
//        return _context.Planes.ToList();
//    }

//    public Plan GetById(int id)
//    {
//        return _context.Planes.FirstOrDefault(p => p.Id == id);
//    }

//    public IEnumerable<Plan> GetByOperator(string operadora)
//    {
//        return _context.Planes.Where(p => p.Operadora == operadora).ToList();
//    }

//    public void Add(Plan plan)
//    {
//        _context.Planes.Add(plan);
//        _context.SaveChanges();
//    }

//    public void Update(Plan plan)
//    {
//        _context.Planes.Update(plan);
//        _context.SaveChanges();
//    }

//    public void Delete(int id)
//    {
//        var plan = GetById(id);
//        if (plan != null)
//        {
//            _context.Planes.Remove(plan);
//            _context.SaveChanges();
//        }
//    }
//}
