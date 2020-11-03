using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace SoftwareDev_Test
{
    class Program
    {
        static void Main()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                // Load Spotify login page
                driver.Navigate().GoToUrl(Tools.ReadConfigValue("MainUrl"));

                #region Login

                Login(driver);

                #endregion

                #region Search Song

                System.Threading.Thread.Sleep(6000); // delay

                List<Song> songList = new List<Song>();
                songList = GetListofSongs(Tools.ReadConfigValue("SongFilePathName"));
                int songNum = 0;
                bool play = false;
                foreach (var song in songList)
                {
                    songNum = songNum+1;
                    System.Threading.Thread.Sleep(5000); // delay

                    SearchSong(driver, song);

                    System.Threading.Thread.Sleep(3000); // delay

                    // To check and play, last song in the list
                    if (songNum == songList.Count)
                    {
                        play = true;
                    }
                    SelectSong(driver, song, play);
                }

                #endregion

                #region Musicplayer Controls
                // Play the selected song
                System.Threading.Thread.Sleep(6000); // delay
                MusicControls(driver);

                #endregion

                #region Create Playlist

                string playListName = Tools.ReadConfigValue("PlayListName");

                System.Threading.Thread.Sleep(10000); // delay

                CreatePlaylist(driver, playListName);

                #endregion

                #region Remove Playlist

                System.Threading.Thread.Sleep(6000); // delay
                RemoveSongFromPlaylist(driver, playListName);

                System.Threading.Thread.Sleep(10000); // delay
                DeletePlaylist(driver, playListName);

                #endregion

                #region Logout

                System.Threading.Thread.Sleep(10000); // delay

                Logout(driver);

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //driver.Quit();
            }
        }

        #region Login/Logout

        /// <summary>
        /// To login to the account
        /// </summary>
        /// <param name="driver"></param>
        static void Login(IWebDriver driver)
        {
            try
            {
                Account account = new Account();

                driver.FindElement(By.Id("login-username")).SendKeys(account.UserName); // username
                driver.FindElement(By.Id("login-password")).SendKeys(account.Password); // password

                System.Threading.Thread.Sleep(3000); // delay
                driver.FindElement(By.Id("login-button")).Click(); // Click login button
            }
            catch (Exception ex)
            {
                throw new Exception("Login() :" + ex.Message);
            }
        }

        /// <summary>
        /// To logout from the account
        /// </summary>
        /// <param name="driver"></param>
        static void Logout(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/settings/account']")).Click();

                System.Threading.Thread.Sleep(1000); // delay
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[2]/div[1]/div/div/section/div/div/div[2]/div[4]/button")).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("Logout() :" + ex.Message);
            }
        }

        #endregion

        #region Search Song

        /// <summary>
        /// To search a song
        /// </summary>
        /// <param name="driver"></param>
        static void SearchSong(IWebDriver driver, Song song)
        {
            try
            {
                System.Threading.Thread.Sleep(2000); // delay

                driver.FindElement(By.XPath("//a[@href='/search']")).Click();

                if ((string.IsNullOrEmpty(song.ArtistName)) || (string.IsNullOrEmpty(song.SongName)))
                {
                    throw new Exception("Search keyword is empty");
                }

                System.Threading.Thread.Sleep(2000); // delay

                driver.FindElement(By.XPath("//*[@id='searchPage']/div/div[1]/label/input")).SendKeys(song.ArtistName);

                System.Threading.Thread.Sleep(2000); // delay

                driver.FindElement(By.XPath("//a[@href='/search/" + song.ArtistName + "/tracks']")).Click();


            }
            catch (Exception ex)
            {
                throw new Exception("SearchSong() :" + ex.Message);
            }
        }

        /// <summary>
        /// To select a song
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="song"></param>
        /// <param name="play"></param>
        static void SelectSong(IWebDriver driver, Song song, bool play = false)
        {
            try
            {

                IWebElement elemSong = driver.FindElement(By.XPath("//*[contains(text(),'" + song.SongName + "')]"));

                // to check whether the song should be added to queue or to play directly
                if (play == false)
                {
                    // Add the song to the queue
                    System.Threading.Thread.Sleep(2000); // delay
                    Actions action = new Actions(driver);
                    action.ContextClick(elemSong).Build().Perform(); // Mouse right click
                    System.Threading.Thread.Sleep(2000); // delay

                    driver.FindElement(By.XPath("//*[@id='main']/div/nav[1]/div[3]")).Click(); // 'Add to Queue'
                }
                else
                {
                    elemSong.Click();
                }

                System.Threading.Thread.Sleep(1000); // delay    
            }
            catch (Exception ex)
            {
                throw new Exception("SelectSong() :" + ex.Message);
            }
        }


        #endregion

        #region Playlist

        /// <summary>
        /// To create a playlist
        /// </summary>
        /// <param name="driver"></param>
        static void CreatePlaylist(IWebDriver driver, string playListName)
        {
            try
            {
                driver.FindElement(By.XPath("//button[contains(.,'Create Playlist')]")).Click(); // to select button

                System.Threading.Thread.Sleep(5000); // delay
                driver.FindElement(By.XPath("//*[@id='main']/div/div[5]/div/div[1]/div/div/input")).SendKeys(playListName); // New Playlist

                System.Threading.Thread.Sleep(3000); // delay
                driver.FindElement(By.XPath("//*[@id='main']/div/div[5]/div/div[2]/div[2]/button")).Click(); // Create

                // Add songs to the playlist
                AddRecomndSongToPlaylist(driver);
            }
            catch (Exception ex)
            {
                throw new Exception("CreatePlaylist() :" + ex.Message);
            }
        }

        /// <summary>
        /// To add recommended songs to the newly created playlist
        /// </summary>
        /// <param name="driver"></param>
        static void AddRecomndSongToPlaylist(IWebDriver driver)
        {
            try
            {
                // Add recommended songs
                System.Threading.Thread.Sleep(5000); // delay
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[2]/div[1]/div/div/div/div/section/div/div/div[2]/div/section/ol/div[1]/div/li/div[3]/button")).Click();

                System.Threading.Thread.Sleep(5000); // delay                               
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[2]/div[1]/div/div/div/div/section/div/div/div[2]/div/section/ol/div[2]/div/li/div[3]/button")).Click();

                System.Threading.Thread.Sleep(5000); // delay                               
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[2]/div[1]/div/div/div/div/section/div/div/div[2]/div/section/ol/div[3]/div/li/div[3]/button")).Click();

            }
            catch (Exception ex)
            {
                throw new Exception("AddRecomndSongToPlaylist() :" + ex.Message);
            }
        }

        /// <summary>
        /// Add specific song to the list
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="song"></param>
        static void AddSongToPlaylist(IWebDriver driver, Song song)
        {
            try
            {
                SearchSong(driver, song);

                IWebElement elemSong = driver.FindElement(By.XPath("//*[contains(text(),'" + song.SongName + "')]"));

                // Add the song to the queue
                System.Threading.Thread.Sleep(1000); // delay
                Actions action = new Actions(driver);
                action.ContextClick(elemSong).Build().Perform(); // Mouse right click
                System.Threading.Thread.Sleep(1000); // delay

                driver.FindElement(By.XPath("//*[@id='main']/div/nav[1]/div[4]")).Click(); // 'Add to Playlist'

                System.Threading.Thread.Sleep(1000); // delay    
                IWebElement elemHover = driver.FindElement(By.XPath("//*[@id='main']/div/div[5]/div/div/div/div[2]/div/div/div/div/div/div/div/div[1]/div/div[1]/div[2]/div"));

                IWebElement elemHoverClick = driver.FindElement(By.XPath("//*[@id='main]/div/div[5]/div/div/div/div[2]/div/div/div/div/div/div/div/div[1]/div/div[2]/div/svg")); // 'Add to Playlist'

                System.Threading.Thread.Sleep(2000); // delay    

                action.MoveToElement(elemHover).Perform();

                System.Threading.Thread.Sleep(1000); // delay
                action.Click(elemHoverClick).Build().Perform();

            }
            catch (Exception ex)
            {
                throw new Exception("AddSongToPlaylist() :" + ex.Message);
            }
        }

        static void RemoveSongFromPlaylist(IWebDriver driver, string playListName)
        {
            try
            {
                List<Song> songList = new List<Song>();
                // Get the list of all playlists
                IList<IWebElement> playlist = driver.FindElements(By.ClassName("Rootlist__playlists-scroll-node"));

                // select the newly created playlist
                playlist.FirstOrDefault(x => x.Text.Equals(playListName)).Click();

                songList = GetListofSongs(Tools.ReadConfigValue("SongsToDeleteFromPlaylistFilePathName"));
                foreach (var song in songList)
                {
                    IWebElement elemSong = driver.FindElement(By.XPath("//*[contains(text(),'" + song.SongName + "')]"));

                    if (elemSong != null)
                    {
                        System.Threading.Thread.Sleep(1000); // delay
                        Actions action = new Actions(driver);
                        action.ContextClick(elemSong).Build().Perform(); // Mouse right click
                        System.Threading.Thread.Sleep(3000); // delay

                        driver.FindElement(By.XPath("//*[@id='main']/div/nav[1]/div[5]")).Click(); // to select button
                    }
                    else
                    {
                        Console.WriteLine("Information: No songs to delete from the playlist");
                    }
                }

                System.Threading.Thread.Sleep(2000); // delay

            }
            catch (Exception ex)
            {
                throw new Exception("RemoveSongFromPlaylist() :" + ex.Message);
            }
        }

        static void DeletePlaylist(IWebDriver driver, string playListName)
        {
            try
            {
                IList<IWebElement> playlist = driver.FindElements(By.ClassName("Rootlist__playlists-scroll-node"));

                // select the newly created playlist
                IWebElement elemSong = playlist.FirstOrDefault(x => x.Text.Equals(playListName));

                if (elemSong != null)
                {
                    System.Threading.Thread.Sleep(1000); // delay
                    Actions action = new Actions(driver);
                    action.ContextClick(elemSong).Build().Perform(); // Mouse right click
                    System.Threading.Thread.Sleep(3000); // delay

                    driver.FindElement(By.XPath("//*[@id='main']/div/nav[4]/div[3]")).Click(); // to select button


                    System.Threading.Thread.Sleep(3000); // delay
                    driver.FindElement(By.XPath(" //*[@id='main']/div/div[5]/div/div/div[2]/button")).Click(); // to select button
                }
                else
                {
                    Console.WriteLine("Information: No songs to delete from the playlist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DeletePlaylist() :" + ex.Message);
            }
        }

        #endregion

        #region Musicplayer Control

        /// <summary>
        /// To manage different musicplayer controls
        /// </summary>
        /// <param name="driver"></param>
        static void MusicControls(IWebDriver driver)
        {
            try
            {
                System.Threading.Thread.Sleep(5000); // delay
                ViewSongQueue(driver);

                System.Threading.Thread.Sleep(5000); // delay
                ChangeSong(driver, Constants.NEXT);

                System.Threading.Thread.Sleep(5000); // delay
                ChangeSong(driver, Constants.PREV);

                System.Threading.Thread.Sleep(5000); // delay
                PlayPauseSong(driver);

            }
            catch (Exception ex)
            {
                throw new Exception("MusicControls() :" + ex.Message);
            }
        }

        /// <summary>
        /// To toggle between play and pause
        /// </summary>
        /// <param name="driver"></param>
        static void PlayPauseSong(IWebDriver driver)
        {
            try
            {   
                // Play // Pause
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[3]/footer/div/div[2]/div/div[1]/div[3]/button")).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("PlayPauseSong() :" + ex.Message);
            }
        }

        /// <summary>
        /// To show the song queue
        /// </summary>
        /// <param name="driver"></param>
        static void ViewSongQueue(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[3]/footer/div/div[3]/div/div/div[1]/div/button")).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("ViewSongQueue() :" + ex.Message);
            }
        }

        /// <summary>
        /// To change songs
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="NextOrPrev"></param>
        static void ChangeSong(IWebDriver driver, string NextOrPrev)
        {
            try
            {
                System.Threading.Thread.Sleep(3000); // delay

                if (NextOrPrev == Constants.NEXT)
                {
                    driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[3]/footer/div/div[2]/div/div[1]/div[4]/button")).Click();
                }
                else if (NextOrPrev == Constants.PREV)
                {
                    driver.FindElement(By.XPath("//*[@id='main']/div/div[4]/div[3]/footer/div/div[2]/div/div[1]/div[2]/button")).Click();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChangeSong() :" + ex.Message);
            }
        }

        #endregion

        #region General

        /// <summary>
        /// To read the songs from the text file and return as Song list
        /// </summary>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        private static List<Song> GetListofSongs(string filePathName)
        {
            DataTable dtSong = new DataTable();
            ReadFromText readFromText = new ReadFromText();
            List<Song> songList = new List<Song>();
            try
            {
                dtSong = readFromText.GetCSVAsTable(filePathName);

                if (dtSong.Rows.Count > 0)
                {

                    foreach (DataRow row in dtSong.Rows)
                    {
                        Song song = new Song
                        {
                            SongName = row[Constants.SONGNAME].ToString(),
                            ArtistName = row[Constants.ARTISTNAME].ToString()
                        };

                        songList.Add(song);
                    }
                }

                return songList;
            }
            catch (Exception ex)
            {
                throw new Exception("GetListofSongs() :" + ex.Message);
            }
            finally
            {
                dtSong.Dispose();
            }
        }

        #endregion
    }
}