using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Pokedex
{
    class Pokemon_class
    {
        public Pokemon_class(string _name, int _lv, JObject _base, JArray _types, int _ID)
        {
            Name = _name;
            HP = (int)_base["HP"];
            Lv = _lv;

            Attack = (int)_base["Attack"];
            Defense = (int)_base["Defense"];
            SpAttack = (int)_base["Sp. Attack"];
            SpDefense = (int)_base["Sp. Defense"];
            Speed = (int)_base["Speed"];

            if (_types.Count == 1)
            {
                Type1 = (Types)Enum.Parse(typeof(Types), (string)_types[0]);
                Type2 = null;
            }
            else
            {
                Type1 = (Types)Enum.Parse(typeof(Types), (string)_types[0]);
                Type2 = (Types)Enum.Parse(typeof(Types), (string)_types[1]);
            }

            ID = _ID;
        }
        //Information about
        public int ID { get; private set; }
        public string Name { get; set; }
        public int Lv { get; set; }

        //Basestats
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }

        //string[] moveset = new string[4]; //replace string with "Attacks" class type // ? how

        //Types
        public Types Type1 { get; set; }
        public Types? Type2 { get; set; }

        public enum Types
        {
            Grass,
            Poison,
            Fire,
            Flying,
            Water,
            Bug,
            Normal,
            Electric,
            Ground,
            Fairy,
            Fighting,
            Psychic,
            Rock,
            Ghost,
            Ice,
            Dragon,
            Steel,
            Dark,
        }

    }

}
