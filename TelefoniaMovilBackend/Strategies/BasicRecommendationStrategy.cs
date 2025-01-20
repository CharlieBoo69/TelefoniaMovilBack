//using TelefoniaMovilBackend.Models;
//using System.Collections.Generic;
//using System.Linq;

//public class BasicRecommendationStrategy : IRecommendationStrategy
//{
//    public IEnumerable<object> RecommendPlans(IEnumerable<Plan> plans, UserPreferences preferences)
//    {
//        // Normalización de datos y cálculo de puntajes
//        return plans.Select(plan => new
//        {
//            Plan = plan,
//            Score = 
//                    (preferences.PesoDatos * ((double)plan.Datos / plans.Max(p => p.Datos))) +
//                    (preferences.PesoMinutos * ((double)plan.Minutos / plans.Max(p => p.Minutos))) +
//                    (preferences.PesoSMS * ((double)plan.SMS / plans.Max(p => p.SMS)))
//        })
//        .OrderByDescending(p => p.Score)
//        .Take(3)
//        .Select(p => new
//        {
//            p.Plan,
//            HybridScore = p.Score
//        });
//    }
//}
