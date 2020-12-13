using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace semestralka_pudilova
{
    public abstract class Place
    {
        private string name; //Parametr název
        private string description; //Parametr popis
        private string code; //Parametr code

        [JsonIgnore]
        public string Name
        {
            get { return name; }
            set
            {
                if ((value.Length >= 2) && (value.Length <= 100)) // Podmínka pro nastavení názvu
                {
                    name = value;
                }
                else
                {
                    throw new Exception("Název musí být dlouhý minimálně 2-100 znaků.");
                }
            }
        }

        [JsonIgnore]
        public string Description
        {
            get { return description; }
            set
            {
                if ((value.Length >= 10) && (value.Length <= 300)) // Podmánka pro nastavení popisu
                {
                    description = value;
                }
                else
                {
                    throw new Exception("Popis musí být dlouhý 10-300 znaků.");
                }
            }
        }

        [JsonProperty("meetingRoom", Order = 0)]
        public string Code
        {
            get { return code; }
            set
            {
                string pattern = @"^[a-zA-Z0-9_.:-]{5,50}$"; //regulární výraz pro code
                if (Regex.IsMatch(value, pattern)) // Podmínka pro zjištění zda code odpovídá požadovanému formátu
                {
                    code = value;
                }
                else
                {
                    throw new Exception("Kód nemá předepsaný formát - musí být dlouhý 5-50 znaků a skládat se z písmen, číslic a speciálních znaků _ . : -");
                }
            }
        }

        public Place(string name, string description, string code) //Konstruktor
        {
            Name = name;
            Description = description;
            Code = code;
        }

        public Place() { }
    }
}
