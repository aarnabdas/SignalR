using System;

namespace Microsoft.AspNet.SignalR.Crank
{
    public class RandomGenerator
    {
        private static Random rand = new Random(DateTime.Now.Second);


        private static string[] firstNames = { "Leanne", "Edward", "Haydee", "Lyle", "Shea", "Curtis", "Roselyn", "Marcus", "Lyn", "Lloyd",
                                      "Isabelle", "Francis", "Olivia", "Roman", "Myong", "Jamie", "Alexis", "Vernon", "Chloe", "Max",
                                      "Kirstie", "Tyler", "Katelin", "Alejandro", "Hannah", "Gavin", "Lynetta", "Russell", "Neida",
                                      "Kurt", "Dannielle", "Aiden", "Janett", "Vaughn", "Michelle", "Brian", "Maisha", "Theo", "Emma",
                                      "Cedric", "Jocelyn", "Darrell", "Grace", "Ivan", "Rikki", "Erik", "Madeleine", "Rufus",
                                      "Florance", "Raymond", "Jenette", "Danny", "Kathy", "Michael", "Layla", "Rolf", "Selma", "Anton",
                                      "Rosie", "Craig", "Victoria", "Andy", "Lorelei", "Drew", "Yuri", "Miles", "Raisa", "Rico",
                                      "Rosanne", "Cory", "Dori", "Travis", "Joslyn", "Austin", "Haley", "Ian", "Liza", "Rickey",
                                      "Susana", "Stephen", "Richelle", "Lance", "Jetta", "Heath", "Juliana", "Rene", "Madelyn", "Stan",
                                      "Eleanore", "Jason", "Alexa", "Adam", "Jenna", "Warren", "Cecilia", "Benito", "Elaine", "Mitch",
                                      "Raylene", "Cyrus" };
        private static string[] lastNames = { "Flinn", "Bryd", "Milligan", "Keesee", "Mercer", "Chapman", "Zobel", "Carter", "Pettey",
                                      "Starck", "Raymond", "Pullman", "Drolet", "Higgins", "Matzen", "Tindel", "Winter", "Charley",
                                      "Schaefer", "Hancock", "Dampier", "Garling", "Verde", "Lenihan", "Rhymer", "Pleiman", "Dunham",
                                      "Seabury", "Goudy", "Latshaw", "Whitson", "Cumbie", "Webster", "Bourquin", "Young", "Rikard",
                                      "Brier", "Luck", "Porras", "Gilmore", "Turner", "Sprowl", "Rohloff", "Magby", "Wallis", "Mullens",
                                      "Correa", "Murphy", "Connor", "Gamble", "Castleman", "Pace", "Durrett", "Bourne", "Hottle",
                                      "Oldman", "Paquette", "Stine", "Muldoon", "Smit", "Finn", "Kilmer", "Sager", "White", "Friedrich",
                                      "Fennell", "Miers", "Carroll", "Freeman", "Hollis", "Neal", "Remus", "Pickering", "Woodrum",
                                      "Bradbury", "Caffey", "Tuck", "Jensen", "Shelly", "Hyder", "Krumm", "Hundt", "Seal", "Pendergast",
                                      "Kelsey", "Milling", "Karst", "Helland", "Risley", "Grieve", "Paschall", "Coolidge", "Furlough",
                                      "Brandt", "Cadena", "Rebelo", "Leath", "Backer", "Bickers", "Cappel" };
        private static string[] lorem = { "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "morbi",
                                      "vulputate", "eros", "ut", "mi", "laoreet", "viverra", "nunc", "lacinia", "non", "condimentum",
                                      "aenean", "lacus", "nisl", "auctor", "at", "tortor", "ac", "fringilla", "sodales", "pretium",
                                      "quis", "iaculis", "in", "aliquam", "ultrices", "felis", "accumsan", "ornare", "etiam",
                                      "elementum", "aliquet", "finibus", "maecenas", "dignissim", "vel", "blandit", "placerat", "sed",
                                      "tempor", "ex", "faucibus", "velit", "nam", "erat", "augue", "quisque", "nulla", "maximus",
                                      "vitae", "e", "lobortis", "euismod", "tristique", "metus", "vehicula", "purus", "diam", "mollis",
                                      "neque", "eu", "porttitor", "mauris", "a", "risus", "orci", "tincidunt", "scelerisque",
                                      "vestibulum", "dui", "ante", "posuere", "turpis", "enim", "cras", "massa", "cursus", "suscipit",
                                      "tempus", "facilisis", "ultricies", "i", "eget", "imperdiet", "donec", "arcu", "ligula",
                                      "sagittis", "hendrerit", "justo", "pellentesque", "mattis", "lacinia", "leo", "est", "magna",
                                      "nibh", "sem", "natoque", "consequat", "proin", "eti", "commodo", "rhoncus", "dictum", "id",
                                      "pharetra", "sapien", "gravida", "sollicitudin", "curabitur", "au", "nisi", "bibendum", "lectus",
                                      "et", "pulvinar"};

        public static string Name()
        {
            return string.Format("{0} {1}", firstNames[rand.Next(0, firstNames.Length - 1)], lastNames[rand.Next(0, lastNames.Length - 1)]);
        }

        public static string Phrase()
        {
            int start = 0;
            int end = 0;
            do {
                start = rand.Next(0, lorem.Length - 1);
                end = rand.Next(0, lorem.Length - 1);
                if (start > end)
                {
                    var aux = start;
                    start = end;
                    end = start;
                }
            } while (start == end);
            var length = end - start;
            if (length > 10)
            {
                length = rand.Next(3, 10);
            }
            return string.Join(" ", SubArray(lorem, start, length));
        }

        public static string[] SubArray(string[] input, int index, int length)
        {
            string[] result = new string[length];
            Array.Copy(input, index, result, 0, length);
            return result;
        }
    }
}
