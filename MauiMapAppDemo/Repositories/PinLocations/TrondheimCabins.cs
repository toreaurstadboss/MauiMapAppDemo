namespace MauiMapAppDemo.Repositories.PinLocations
{
    public static partial class TrondheimCabins
    {

        public static List<Cabin> GetSampleData()
        {

            return new List<Cabin>()
                {

                    new Cabin(
                      "Estenstadhytta",
                      "Popular cabin in Estenstadmarka",
                       63.39478511341777, 10.488396418945399),

                    new Cabin(
                    "Elgsethytta",
                    "Traditional cabin in Bymarka",
                       63.42046109744264, 10.21251573534687),

                    new Cabin(
                    "Skistua",
                    "Gateway to Bymarka",
                     63.41789876758627, 10.26377206090971),

                    new Cabin(
                    "Grønlia",
                    "Popular coffee stop",
                     63.40284632068321, 10.243509429760273),

                    new Cabin(
                    "Rønningen",
                    "Family-friendly cabin",
                    63.37881115331552, 10.262531628360053)
                };
        }
    }

}

               


            

