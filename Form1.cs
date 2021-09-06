using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AVL_BST_lib;
using Password_Hash_lib;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace AdvancedMusicPlayer
{
    public partial class MusicPlayer : Form
    {
        public MusicPlayer()
        {
            InitializeComponent();
        }

        int count;
        bool doubleClicked;
        bool sortedByArtist;
        bool deleteSong;
        bool searchedOneArtist;
        string selectedSong;
        char[] trimChars = { '.', 'm', 'p', '3' };

        EditSong editSong;
        Artist currentArtist;

        Tree<Song> bst = new Tree<Song>();
        List<Artist> artistList = new List<Artist>();
        public static UserStorage users = new UserStorage();

        private void addButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Multiselect = true;

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFile.FileNames)
                    {
                        if (file.EndsWith(".mp3"))
                        {
                            Song newSong = new Song();
                            newSong.fileName = file;
                            newSong.name = Path.GetFileName(newSong.fileName).TrimEnd(trimChars);

                            tryFindArtist("unknown", newSong);
                            bst.insert(newSong);
                        }
                    }
                    
                    displayPlaylist();
                }
            }
        }

        private void songSortButton_Click(object sender, EventArgs e)
        {
            sortedByArtist = false;
            searchedOneArtist = false;
            displayPlaylist();
        }

        private void artistSortButton_Click(object sender, EventArgs e)
        {
            sortedByArtist = true;
            searchedOneArtist = false;
            displayPlaylist();
        }

        private void songListBox_Click(object sender, EventArgs e)
        {
            count = 0;
            doubleClicked = false;
            int index = songListBox.SelectedIndex + 1;

            decideSearchMethod(index);
        }

        private void songListBox_DoubleClick(object sender, EventArgs e)
        {
            count = 0;
            doubleClicked = true;
            int index = songListBox.SelectedIndex + 1;

            editSong = new EditSong();

            if (editSong.ShowDialog() == DialogResult.OK)
            {
                decideSearchMethod(index);
            }

            displayPlaylist();
            doubleClicked = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            count = 0;
            deleteSong = true;
            int index = songListBox.SelectedIndex + 1;

            decideSearchMethod(index);

            displayPlaylist();
            deleteSong = false;
        }

        private void searchSongButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(songNameBox.Text))
            {
                count = 0;
                string songName = songNameBox.Text;

                Song temp = new Song();
                temp.name = songName;

                if (searchedOneArtist)
                {
                    searchArtist(temp);
                }
                else
                {
                    searchTree(temp);
                }
            }
        }

        private void searchArtistButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(artistNameBox.Text))
            {
                Artist temp = new Artist();
                temp.name = artistNameBox.Text;
                currentArtist = BinarySearch<Artist>.binarySearch(artistList, temp);

                if (currentArtist == null || currentArtist.songs.Count == 0)
                {
                    MessageBox.Show("No songs by " + artistNameBox.Text + " were found.");
                }
                else
                {
                    searchedOneArtist = true;
                    sortedByArtist = false;
                    displayOneArtist(currentArtist);
                }
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(selectedSong))
            {
                mediaPlayer.URL = selectedSong;
            }
            else
            {
                MessageBox.Show("Cannot find file.");
            }
        }


        // // // Methods for checking if an artist exists and if the new song already exists in their song list:

        // tries to find an artist when adding or editing a song and creates new artist if none is found.
        public Artist tryFindArtist(string enteredName, Song newSong)
        {
            Artist newArtist = new Artist();
            newArtist.name = enteredName;

            Artist artist = BinarySearch<Artist>.binarySearch(artistList, newArtist);

            if (artist != null)
            {
                newSong.artist = artist;
                tryFindSong(newSong, artist);

                return artist;
            }
            else
            {
                newArtist.songs.Add(newSong);
                newSong.artist = newArtist;

                artistList.Add(newArtist);
                MergeSort<Artist>.mergeSort(artistList);

                return newArtist;
            }
        }

        private void tryFindSong(Song newSong, Artist artist)
        {
            Song song = BinarySearch<Song>.binarySearch(artist.songs, newSong);
            
            if (song == null)
            {
                artist.songs.Add(newSong);
                MergeSort<Song>.mergeSort(artist.songs);
            }
        }


        // // // Methods for displaying the songs by choice of sort:

        // display the playlist based on choice to sort the playlist by song, artist or one artist
        public void displayPlaylist()
        {
            songListBox.Items.Clear();
            
            if (searchedOneArtist)
            {
                displayOneArtist(currentArtist);
            }
            else if (sortedByArtist)
            {
                displayByArtist();
            }
            else
            {
                displayBySong();
            }
        }

        // iterates through each artist's song list and adds each song to the listbox.
        public void displayByArtist()
        {
            foreach (Artist artist in artistList)
            {
                foreach (Song song in artist.songs)
                {
                    songListBox.Items.Add(artist.name + " - " + song.name);
                }
            }
        }

        public void displayBySong()
        {
            if (bst.root != null)
            {
                ascending(bst.root);
            }
        }

        // recursively traverses the binary tree from lowest to highest and adds each song to the listbox.
        public void ascending(Node<Song> node)
        {
            if (node.left != null)
            {
                ascending(node.left);
            }

            songListBox.Items.Add(node.data.name + " - " + node.data.artist.name);

            if (node.right != null)
            {
                ascending(node.right);
            }
        }

        // displays only the searched artist's songs in the listbox.
        public void displayOneArtist(Artist artist)
        {
            if (artistList.Count > 0)
            {
                songListBox.Items.Clear();

                foreach (Song song in artist.songs)
                {
                    songListBox.Items.Add(artist.name + " - " + song.name);
                }
            }
        }


        // // // Methods to handle the selection of a song in the listbox:

        public void decideSearchMethod(int index)
        {
            if (searchedOneArtist)
            {
                findSongOfArtist(index);
            }
            else if (sortedByArtist)
            {
                findArtistAndSong(index);
            }
            else
            {
                findNodeOfIndex(bst.root, index);
            }
        }

        // iterates through the binary tree until it finds the node for the song selected in the listbox.
        public void findNodeOfIndex(Node<Song> node, int index)
        {
            if (node == null)
            {
                return;
            }

            if (count <= index)
            {
                findNodeOfIndex(node.left, index);
                count++;

                if (count == index)
                {
                    handleSelection(node.data);
                }

                findNodeOfIndex(node.right, index);
            }
            else return;
        }

        // iterates through each artist's song list until it finds the song selected in the listbox.
        public void findArtistAndSong(int index)
        {
            foreach (Artist artist in artistList)
            {
                foreach (Song song in artist.songs)
                {
                    count++;

                    if (count == index)
                    {
                        handleSelection(song);
                        return;
                    }
                }
            }
        }

        // iterates through the current artist's song list until it finds the song selected in the listbox.
        public void findSongOfArtist(int index)
        {
            foreach (Song song in currentArtist.songs)
            {
                count++;

                if (count == index)
                {
                    handleSelection(song);
                    return;
                }
            }
        }

        // handle the choice to either delete, edit or select the song.
        public void handleSelection(Song song)
        {
            if (deleteSong)
            {
                song.artist.songs.Remove(song);
                bst.delete(song);

                songNameBox.Clear();
                artistNameBox.Clear();

                selectedSong = null;
                return;
            }

            if (doubleClicked)
            {
                Song newSong = new Song();
                editSongAndArtist(newSong, song);
                setTextboxText(newSong);
                return;
            }
            else
            {
                selectedSong = song.fileName;
                setTextboxText(song);
                return;
            }
        }

        // edits the song and artist based on the data from the edit dialog.
        public void editSongAndArtist(Song newSong, Song song)
        {
            newSong.fileName = song.fileName;

            if (!string.IsNullOrEmpty(editSong.songTextbox.Text))
            {
                newSong.name = editSong.songTextbox.Text;
            }
            else
            {
                newSong.name = song.name;
            }

            if (!string.IsNullOrEmpty(editSong.artistTextbox.Text))
            {
                tryFindArtist(editSong.artistTextbox.Text, newSong);
            }
            else
            {
                newSong.artist = song.artist;
                newSong.artist.songs.Add(newSong);
            }

            song.artist.songs.Remove(song);
            bst.delete(song);
            bst.insert(newSong);

            setTextboxText(newSong);
        }

        // sets the textbox texts to the name and artist of the song selected in the listbox
        public void setTextboxText(Song song)
        {
            songNameBox.Text = song.name;
            artistNameBox.Text = song.artist.name;
        }


        // // // Methods for searching for song or artist:

        // searches for a song in the binary tree.
        public void searchTree(Song song)
        {
            Node<Song> node = bst.search(song);

            if (node == null)
            {
                MessageBox.Show(songNameBox.Text + " not found.");
            }
            else if (!sortedByArtist)
            {
                findIndexOfNode(bst.root, node.data);
                songNameBox.Text = node.data.name;
                artistNameBox.Text = node.data.artist.name;
            }
            else
            {
                findIndexOfSong(node.data);
                songNameBox.Text = node.data.name;
                artistNameBox.Text = node.data.artist.name;
            }
        }

        // finds the index in the listbox for the node in the binary tree of the song searched when the playlist is sorted by song.
        public void findIndexOfNode(Node<Song> node, Song song)
        {
            if (node == null)
            {
                return;
            }

            findIndexOfNode(node.left, song);
            count++;

            if (node.data.Equals(song))
            {
                songListBox.SelectedIndex = count - 1;
                selectedSong = song.fileName;
                return;
            }

            findIndexOfNode(node.right, song);
        }

        // finds the index in the listbox for the song searched when the playlist is sorted by artist.
        public void findIndexOfSong(Song search)
        {
            foreach (Artist artist in artistList) 
            {
                foreach (Song song in artist.songs)
                {
                    if (song.CompareTo(search) == 0)
                    {
                        songListBox.SelectedIndex = count;
                        selectedSong = song.fileName;
                        return;
                    }

                    count++;
                }
            }
        }

        // finds the index in the listbox for the searched song of a particlar artist when the listbox displays only that artist.
        public void searchArtist(Song search)
        {
            bool found = false;

            foreach (Song song in currentArtist.songs)
            {
                if (song.CompareTo(search) == 0)
                {
                    songListBox.SelectedIndex = count;
                    selectedSong = song.fileName;
                    found = true;
                }

                count++;
            }

            if (!found)
            {
                MessageBox.Show(songNameBox.Text + " not found.");
            }
        }


        // // // Sign in, open file and save file methods:

        private void signInButton_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                if (loginForm.loginResult == true)
                {
                    openButton.Enabled = true;
                    addButton.Enabled = true;
                    saveButton.Enabled = true;
                    signInButton.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Login failed");
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    string filePath = Path.GetFullPath(saveFile.FileName);

                    using (var writer = new StreamWriter(filePath))
                    {
                        useCsvWriter(writer);
                    }
                }
            }
        }

        public void useCsvWriter(StreamWriter writer)
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<SongMap>();
                csv.WriteHeader<Song>();
                csv.NextRecord();
                writeSongs(bst.root, csv);
            }
        }

        public void writeSongs(Node<Song> node, CsvWriter csv)
        {
            if (node.left != null)
            {
                writeSongs(node.left, csv);
            }

            csv.WriteRecord(node.data);
            csv.NextRecord();

            if (node.right != null)
            {
                writeSongs(node.right, csv);
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string filePath = Path.GetFullPath(openFile.FileName);

                    using (var reader = new StreamReader(filePath))
                    {
                        loadSongs(reader);
                    }
                }
            }
        }

        public void loadSongs(StreamReader reader)
        {
            try
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<SongMap>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        Song newSong = csv.GetRecord<Song>();
                        bst.insert(newSong);
                        tryFindArtist(newSong.artist.name, newSong);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load file.");
            }
            
            displayPlaylist();
        }

        private void MusicPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (FileStream fs = new FileStream("users.dat", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (User user in UserStorage.users)
                    {
                        bw.Write(user.UserID);
                        bw.Write(user.PasswordHash);
                        bw.Write(user.Salt);
                    }
                }
            }
        }

        private void MusicPlayer_Load(object sender, EventArgs e)
        {
            string usersFile = "users.dat";

            if (File.Exists(usersFile))
            {
                using (FileStream fs = new FileStream(usersFile, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        while (fs.Position < fs.Length)
                        {
                            User user = new User();

                            user.UserID = br.ReadString();
                            user.PasswordHash = br.ReadString();
                            user.Salt = br.ReadString();

                            users.AddUser(user);
                        }
                    }
                }
            }
        }
    }

    // maps the song and user class properties as headers in the csv file. 
    public sealed class SongMap : ClassMap<Song>
    {
        public SongMap()
        {
            Map(member => member.name).Name("song");
            Map(member => member.fileName).Name("file");
            Map(member => member.artist.name).Name("artist");
        }
    }

    public sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(member => member.UserID).Name("userid");
            Map(member => member.PasswordHash).Name("pwHash");
            Map(member => member.Salt).Name("salt");
        }
    }
}
