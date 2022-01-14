using System;
using System.Collections.Generic;
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
using System.IO;


namespace MusicList
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //comboBoxMusic.ItemsSource = arrayMonth;
            string filePath;

            filePath = @"ProgramData.txt";
            if (!File.Exists(filePath))
            {
                MusicList musicList = new MusicList();
                musicList.listMusicNames = new List<string>();

                using (FileStream fs = File.Create(filePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(musicList));
                    fs.Write(info, 0, info.Length);
                }
            }

            UpdateList();





        }

        class MusicList
        {
            public List<string> listMusicNames { get; set; }
        }

        class MusicData
        {
            public string Name { get; set; }
            public string FullName { get; set; }
            public string URL { get; set; }
        }

        private void UpdateList()
        {
            string filePath, jsonContent;

            filePath = @"ProgramData.txt";
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    //byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                    //fs.Write(info, 0, info.Length);
                }
            }

            comboBoxSong.Items.Clear();

            jsonContent = System.IO.File.ReadAllText(filePath);
            MusicList deserializedMusicList = JsonConvert.DeserializeObject<MusicList>(jsonContent);


            foreach (string item in deserializedMusicList.listMusicNames)
            {
                comboBoxSong.Items.Add(item);
            }


        }



        private void btnAddSong_Click(object sender, RoutedEventArgs e)
        {
            string filePath, name, fullName, url, jsonList;
            filePath = @"ProgramData.txt";

            try
            {
                MusicData musicData = new MusicData();

                jsonList = File.ReadAllText(filePath);
                MusicList musicList = JsonConvert.DeserializeObject<MusicList>(jsonList);



                name = txtAddSongName.Text;
                fullName = txtAddSongFullName.Text;
                url = txtAddSongURL.Text;

                musicList.listMusicNames.Add(name);

                musicData.Name = name;
                musicData.FullName = fullName;
                musicData.URL = url;


                using (FileStream fs = File.Create(filePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(musicList));
                    fs.Write(info, 0, info.Length);
                }



                using (FileStream fs = File.Create($"{name.Replace(" ", "_")}.json"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(musicData));
                    fs.Write(info, 0, info.Length);
                }
                UpdateList();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void btnDeleteSong_Click(object sender, RoutedEventArgs e)
        {
            string filePath, fileReadAll, nameFile, replacedReadAll;

            filePath = @"ProgramData.txt";

            nameFile = $"{txtAddSongName.Text.Replace(" ", "_")}.txt";
            File.Delete(nameFile);

            fileReadAll = File.ReadAllText(filePath);
            replacedReadAll = fileReadAll.Replace($"{txtAddSongName.Text}|{txtAddSongFullName.Text}|{txtAddSongURL.Text}", "");
            using (FileStream fs = File.Create(filePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(replacedReadAll);
                fs.Write(info, 0, info.Length);
            }


            UpdateList();
            txtAddSongFullName.Text = "";
            txtAddSongName.Text = "";
            txtAddSongURL.Text = "";

        }

        private void comboBoxSong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item, nameFile, nameFileData;


            try
            {
                if (comboBoxSong.SelectedItem == null)
                {

                }


                else
                {
                    item = comboBoxSong.SelectedItem.ToString();

                    nameFile = $"{item.Replace(" ", "_")}.json";
                    nameFileData = File.ReadAllText(nameFile);

                    MusicData deserializedMusicData = JsonConvert.DeserializeObject<MusicData>(nameFileData);

                    lblReadName.Content = deserializedMusicData.Name;
                    lblReadURL.Content = deserializedMusicData.URL;





                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void lblReadURL_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(lblReadURL.Content.ToString());
        }

        private void lblReadSong_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtAddSongFullName.Text = string.Empty;
            txtAddSongName.Text = string.Empty;
            txtAddSongURL.Text = string.Empty;

            lblReadName.Content = string.Empty;
            lblReadURL.Content = string.Empty;

            comboBoxSong.SelectedItem = null;
        }
    }
}
