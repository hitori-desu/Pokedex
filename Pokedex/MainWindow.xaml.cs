using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokeApiNet;
using System.Globalization;
using System.Threading;

namespace Pokedex
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Pokemon_class> national = new ObservableCollection<Pokemon_class>();
        ObservableCollection<Pokemon_class> kanto = new ObservableCollection<Pokemon_class>();
        ObservableCollection<Pokemon_class> johto = new ObservableCollection<Pokemon_class>();
        ObservableCollection<Pokemon_class> hoenn = new ObservableCollection<Pokemon_class>();
        ObservableCollection<Pokemon_class> sinnoh = new ObservableCollection<Pokemon_class>();
        ObservableCollection<Pokemon_class> unova = new ObservableCollection<Pokemon_class>();

        List<BitmapImage> img_list = new List<BitmapImage>();
        List<BitmapImage> img_shiny_list = new List<BitmapImage>();

        static int COUNT = 0;
        int count_cache = COUNT;

        public static dynamic ReadJsonFile(string jsonFileIn)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            return jsonFile;
        }
        static void CreatePokemon(ObservableCollection<Pokemon_class> _national, ObservableCollection<Pokemon_class> _kanto, ObservableCollection<Pokemon_class> _johto, ObservableCollection<Pokemon_class> _hoenn, ObservableCollection<Pokemon_class> _sinnoh, ObservableCollection<Pokemon_class> _unova)
        {
            dynamic jsonFile = ReadJsonFile("../../pokemon.json");

            Pokemon_class[] pokemonarray = new Pokemon_class[649];

            int a = 0;
            for (int i = 0; i < 649; i++)
            {
                pokemonarray[i] = new Pokemon_class((string)jsonFile[i]["name"]["english"], 1, jsonFile[i]["base"], jsonFile[i]["type"], a += 1);
                if (i < 151)
                {
                    _kanto.Add(pokemonarray[i]);
                }
                else if (i > 150 && i < 251)
                {
                    _johto.Add(pokemonarray[i]);
                }
                else if (i > 250 && i < 386)
                {
                    _hoenn.Add(pokemonarray[i]);
                }
                else if (i > 385 && i < 494)
                {
                    _sinnoh.Add(pokemonarray[i]);
                }
                else if (i > 493 && i < 649)
                {
                    _unova.Add(pokemonarray[i]);
                }
                _national.Add(pokemonarray[i]);
            }
        }

        public MainWindow()
        {
            CreatePokemon(national, kanto, johto, hoenn, sinnoh, unova);
            InitializeComponent();
            cb_regions.ItemsSource = Enum.GetValues(typeof(Regions));
            dg_pkmn.ItemsSource = national;

        }

        private async void dg_pkmn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbox_index.Text = "Loading...";

            DirectoryInfo forms = new DirectoryInfo(@"../../sprites/forms");
            List<BitmapImage> img_cache = new List<BitmapImage>();
            List<BitmapImage> img_shiny_cache = new List<BitmapImage>();

            Pokemon_class fortnite = (Pokemon_class)dg_pkmn.SelectedItem;

            img_cache.Clear();
            img_shiny_cache.Clear();
            if (dg_pkmn.SelectedIndex != -1)
            {
                DirectoryInfo img_dir = new DirectoryInfo($@"../../sprites/forms/{fortnite.Name.ToLower()}");
                DirectoryInfo img_shiny_dir = new DirectoryInfo($@"../../sprites/forms/{fortnite.Name.ToLower()}/shiny");

                foreach (var item in forms.GetDirectories())
                {
                    if (item.Name.ToLower() == fortnite.Name.ToLower())
                    {
                        //img_cache.Add(new BitmapImage(new Uri($@"./sprites/{fortnite.ID}.png", UriKind.Relative)));
                        //img_shiny_cache.Add(new BitmapImage(new Uri($@"./sprites/shiny/{fortnite.ID}s.png", UriKind.Relative)));
                        foreach (var item2 in img_dir.GetFiles())
                        {
                            img_cache.Add(new BitmapImage(new Uri($@"{img_dir}/{item2}", UriKind.Relative)));

                        }
                        foreach (var item3 in img_shiny_dir.GetFiles())
                        {
                            img_shiny_cache.Add(new BitmapImage(new Uri($@"{img_dir}/shiny/{item3}", UriKind.Relative)));
                        }
                        img_pkmn.Source = new BitmapImage(new Uri($@"./sprites/{fortnite.ID}.png", UriKind.Relative));
                        img_shiny.Source = new BitmapImage(new Uri($@"./sprites/shiny/{fortnite.ID}s.png", UriKind.Relative));
                        lbl_index.Content = $"1/{img_cache.Count}";
                        break;
                    }
                    else
                    {
                        img_pkmn.Source = new BitmapImage(new Uri($@"./sprites/{fortnite.ID}.png", UriKind.Relative));
                        img_shiny.Source = new BitmapImage(new Uri($@"./sprites/shiny/{fortnite.ID}s.png", UriKind.Relative));
                        lbl_index.Content = "1/1";
                    }
                    
                }
                img_list = img_cache;
                img_shiny_list = img_shiny_cache;
                await GetPokemon(fortnite.Name.ToLower());
                COUNT = 0;

            }

        }

        private void tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            dg_pkmn.SelectedIndex = -1;
            ObservableCollection<Pokemon_class> filtered_list = new ObservableCollection<Pokemon_class>();
            ObservableCollection<Pokemon_class> cache = new ObservableCollection<Pokemon_class>();
            switch (cb_regions.SelectedItem.ToString().ToLower())
            {
                case "national":
                    cache = national;
                    break;
                case "kanto":
                    cache = kanto;
                    break;
                case "johto":
                    cache = johto;
                    break;
                case "hoenn":
                    cache = hoenn;
                    break;
                case "sinnoh":
                    cache = sinnoh;
                    break;
                case "unova":
                    cache = unova;
                    break;

            }
            foreach (Pokemon_class item in cache)
            {
                if (item.Name.ToLower().StartsWith(tb_search.Text.ToLower()))
                {
                    filtered_list.Add(item);
                }
                else
                {
                    continue;
                }
            }
            dg_pkmn.ItemsSource = filtered_list;
        }
        async Task GetPokemon(string _name)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            PokeApiClient pokeClient = new PokeApiClient();
            Pokemon stats = await pokeClient.GetResourceAsync<Pokemon>($"{_name}");
            PokemonSpecies current = await pokeClient.GetResourceAsync<PokemonSpecies>($"{stats.Species.Name}");
            var filtered = current.FlavorTextEntries.Where(e => e.Language.Name == "en").ToList();
            string cache = filtered[0].FlavorText;
            cache = cache.Replace("\n", " ").Replace("\f", " ").Replace("POKéMON", "Pokemon");
            tbox_index.Text = $"Number: #{stats.Id}\n\nName: {textInfo.ToTitleCase(stats.Name)}\n\nWeight: {stats.Weight / 10} kg\n\nHeight: {stats.Height * 10} cm\n\nEntry:\n{cache}";

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_search.Text = String.Empty;
            switch (cb_regions.SelectedIndex)
            {
                case 0:
                    dg_pkmn.ItemsSource = national;
                    break;
                case 1:
                    dg_pkmn.ItemsSource = kanto;
                    break;
                case 2:
                    dg_pkmn.ItemsSource = johto;
                    break;
                case 3:
                    dg_pkmn.ItemsSource = hoenn;
                    break;
                case 4:
                    dg_pkmn.ItemsSource = sinnoh;
                    break;
                case 5:
                    dg_pkmn.ItemsSource = unova;
                    break;

            }

        }

        enum Regions
        {
            National = 0,
            Kanto = 1,
            Johto = 2,
            Hoenn = 3,
            Sinnoh = 4,
            Unova = 5,
        }

        private void btn_left_Click(object sender, RoutedEventArgs e)
         {
            try
            {
                COUNT -= 1;
                img_pkmn.Source = img_list[COUNT];
                img_shiny.Source = img_shiny_list[COUNT];
                count_cache = COUNT;
                lbl_index.Content = $"{COUNT+1}/{img_list.Count}";
            }
            catch (ArgumentOutOfRangeException)
            {
                COUNT = count_cache;
                
            }


        }

        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                COUNT += 1;
                img_pkmn.Source = img_list[COUNT];
                img_shiny.Source = img_shiny_list[COUNT];
                count_cache = COUNT;
                lbl_index.Content = $"{COUNT+1}/{img_list.Count}";
            }
            catch (ArgumentOutOfRangeException)
            {
                COUNT = count_cache;
            }


        }
    }
}
