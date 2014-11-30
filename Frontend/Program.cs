/* Program.cs
 *
 * Contains the Frontend UI driver.
 *
 * Copyright (C) 2014 - Blake Lowe, Jordan Polaniec
 *
 * This file is part of Map My Garage Sale.
 * 
 * Map My Garage Sale is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * Map My Garage Sale is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Map My Garage Sale. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Gtk;

namespace Frontend
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			Application.Init ();
            MainWindow win = new MainWindow (); 

            GLib.ExceptionManager.UnhandledException += (GLib.UnhandledExceptionArgs e) => {
                Exception exc = (Exception)e.ExceptionObject;
                win.Destroy();

                using(MessageDialog md = new MessageDialog(win, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                    string.Format("Error.  Send a screenshot of this to the devs so they can fix it: \n{0} \n{1}", exc.Message, exc.TargetSite)))
                {
                    md.Run();
                    md.Destroy();
                    win.Destroy();
                    System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("main.exe");
                    if(proc.Length > 0)
                    {
                        foreach(System.Diagnostics.Process p in proc)
                        {
                            p.Kill(); //will kill main.exe
                        }
                    }
                }
            };
            win.ShowAll ();
            Application.Run ();
		}
       
	}
}
