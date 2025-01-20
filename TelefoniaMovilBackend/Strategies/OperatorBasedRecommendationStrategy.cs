//using TelefoniaMovilBackend.Models;

//public class OperatorBasedRecommendationStrategy : IRecommendationStrategy
//{
//    public IEnumerable<object> RecommendPlans(IEnumerable<Plan> plans, UserPreferences preferences)
//    {
//        // Filtrar por operadora si está definida en las preferencias
//        var filteredPlans = !string.IsNullOrEmpty(preferences.Operadora)
//            ? plans.Where(p => p.Operadora == preferences.Operadora)
//            : plans;

//        return filteredPlans.Select(plan => new
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
