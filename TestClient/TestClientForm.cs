using DotNetty.Transport.Channels;
using Serilog;
using EzDotNetty.Bootstrap.Client;
using TestClientShared.NetworkHandler;
using TestClientShared.Util;

namespace TestClient
{
    public partial class TestClientForm : Form
    {
        public IChannel? Channel { get; set; }

        public ClientHandler? ClientHandler { get; set; }

        public IChannelHandlerContext? ChannelHandlerContext { get; set; }

        private readonly Dictionary<int, PlayerControl> PlayerControls = new();

        private int? MyPlayerIndex { get; set; }

        public TestClientForm()
        {
            InitializeComponent();

#pragma warning disable 8622 // null 허용 여부로 인한 경고 제거
            FormClosing += new FormClosingEventHandler(OnClosing);
#pragma warning restore 8622
        }

        private async Task CloseSocket()
        {
            if (Channel == null)
                return;

            await Channel!.CloseAsync();
            Channel = null;
        }

        public void OnConnect(IChannelHandlerContext context)
        {
            ChannelHandlerContext = context;
            OnLog($"OnConnect() <Context:{context}>");

            var action = () =>
            {
                this.buttonDisconnect.Enabled = true;
                this.buttonConnect.Enabled = false;

                this.buttonLogin.Enabled = true;
                this.textBoxId.Enabled = true;
            };

            InvokeAction(action);
        }

        public void OnClose(IChannelHandlerContext context)
        {
            OnLog($"OnClose() <Context:{context}>");
            Channel = null;
            if (context == ChannelHandlerContext)
            {
                ChannelHandlerContext = null;
            }

            var action = () =>
            {
                foreach (var playerControl in PlayerControls.Values)
                {
                    this.Controls.Remove(playerControl.Control);
                }

                PlayerControls.Clear();

                this.buttonDisconnect.Enabled = false;
                this.buttonConnect.Enabled = true;

                this.textBoxId.Enabled = false;
                this.buttonLogin.Enabled = false;
                this.buttonLogout.Enabled = false;

                this.textBoxRoom.Enabled = false;
                this.buttonEnter.Enabled = false;
                this.buttonLeave.Enabled = false;
            };

            InvokeAction(action);
        }

        private void InvokeAction(Action action)
        {
            if (this.IsDisposed || this.Disposing)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public void OnLog(string message)
        {
            if (this.Disposing || this.IsDisposed)
                return;

            Log.Logger.Information(message);

            var action = () => listBoxLog.Items.Add(message);
            if (listBoxLog.InvokeRequired)
            {
                listBoxLog.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public void Send<T>(T t) where T : Protocols.Request.Header
        {
            ChannelHandlerContext?.WriteAndFlushAsync(t.ToByteBuffer());
        }

        private void SendMove(Protocols.Common.Vector3 vector3)
        {
            Send(new Protocols.Request.Move { Position = vector3 });
        }

        #region WinformHandler

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (Channel != null)
                return;

            Channel = await BootstrapHelper.RunClientAsync<ClientHandler>((handler) =>
            {
                if (ClientHandler?.ClientDispatcher != null)
                {
                    ClientHandler!.ClientDispatcher.Release();
                }

                handler.OnConnect = OnConnect;
                handler.OnClose = OnClose;

                var dispatcher = new TestClientPacketDispatcher
                {
                    OnLog = OnLog,
                    TestClientForm = this,
                };

                handler.ClientDispatcher = dispatcher;
                ClientHandler = handler;
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!MyPlayerIndex.HasValue)
                return base.ProcessCmdKey(ref msg, keyData);

            if (!PlayerControls.TryGetValue(MyPlayerIndex.Value, out var control))
            {
                OnLog($"ProcessCmdKey() <Desc:Not found CustomControls> <PlayerIndex:{MyPlayerIndex}>");
                return false;
            }

            var originPosition = control.Position;
            switch (keyData)
            {
                case Keys.Left:
                    SendMove(new Protocols.Common.Vector3 { x = originPosition.x - 0.5f, z = originPosition.z });
                    return true;
                case Keys.Right:
                    SendMove(new Protocols.Common.Vector3 { x = originPosition.x + 0.5f, z = originPosition.z });
                    return true;
                case Keys.Up:
                    SendMove(new Protocols.Common.Vector3 { x = originPosition.x, z = originPosition.z - 0.5f });
                    return true;
                case Keys.Down:
                    SendMove(new Protocols.Common.Vector3 { x = originPosition.x, z = originPosition.z + 0.5f });
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            ClientHandler?.ClientDispatcher?.Release();
            Task.WhenAll(CloseSocket(), BootstrapHelper.GracefulCloseAsync());
        }
        private async void buttonDisconnect_Click(object sender, EventArgs e)
        {
            await CloseSocket();
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            if(!int.TryParse( textBoxRoom.Text, out var roomId))
            {
                MessageBox.Show($"방 번호는 숫자만 가능합니다");
                return;
            }

            Send(new Protocols.Request.Enter { RoomId = roomId });
        }

        private void buttonLeave_Click(object sender, EventArgs e)
        {
            Send(new Protocols.Request.Leave { });
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Send(new Protocols.Request.Login { Name = textBoxId.Text });
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Send(new Protocols.Request.Logout { });
        }
        #endregion WinformHandler

        #region PacketHandler
        public void OnLogin(Protocols.Response.Login login)
        {
            var action = () =>
            {
                this.textBoxId.Enabled = false;
                this.buttonLogout.Enabled = true;
                this.buttonLogin.Enabled = false;

                this.textBoxRoom.Enabled = true;
                this.buttonEnter.Enabled = true;
            };

            InvokeAction(action);
        }

        public void OnLogout(Protocols.Response.Logout logout)
        {
            var action = () =>
            {
                this.textBoxId.Enabled = true;
                this.buttonLogin.Enabled = true;
                this.buttonLogout.Enabled = false;

                this.textBoxRoom.Enabled = false;
                this.buttonEnter.Enabled = false;
                this.buttonLeave.Enabled = false;
            };

            InvokeAction(action);
        }

        public void OnEnter(Protocols.Response.Enter enter)
        {
            if(textBoxId.Text == enter.Name)
            {
                MyPlayerIndex = enter.PlayerIndex;
            }

            var action = () =>
            {
                var label = new Label
                {
                    Location = ToPoint(enter.Position),
                    Name = enter.Name,
                    Size = new Size(30, 30),
                    Text = enter.Name,
                    ForeColor = MyPlayerIndex == enter.PlayerIndex ? Color.Red : Color.Blue,
                };

                PlayerControls.Add(enter.PlayerIndex, new PlayerControl
                {
                    Control = label,
                    Position = enter.Position,
                });

                this.textBoxRoom.Enabled = false;
                this.buttonEnter.Enabled = false;
                this.buttonLeave.Enabled = true;

                this.Controls.Add(label);
                this.Controls.SetChildIndex(label, 0);
            };

            InvokeAction(action);
        }

        public void OnLeave(Protocols.Response.Leave leave)
        {
            if (MyPlayerIndex == leave.PlayerIndex)
            {
                MyPlayerIndex = null;
            }

            var action = () =>
            {
                if (PlayerControls.TryGetValue(leave.PlayerIndex, out var control))
                {
                    this.Controls.Remove(control.Control);
                    PlayerControls.Remove(leave.PlayerIndex);
                }

                this.textBoxRoom.Enabled = true;
                this.buttonEnter.Enabled = true;
                this.buttonLeave.Enabled = false;
            };

            InvokeAction(action);
        }

        public void OnMove(Protocols.Response.Move move)
        {
            var action = () =>
            {
                if (PlayerControls.TryGetValue(move.PlayerIndex, out var control))
                {
                    control.Control!.Location = ToPoint(move.Position);
                    control.Position = move.Position;
                }
            };

            InvokeAction(action);
        }

        #endregion PacketHandler

        private Point ToPoint(Protocols.Common.Vector3 vector3)
        {
            // 30f는 임시 좌표 확대값임.
            return new Point(playAreaGroupBox.Location.X + playAreaGroupBox.Size.Width / 2 + (int)(vector3.x * 30f),
                 playAreaGroupBox.Location.Y + playAreaGroupBox.Size.Height / 2 + (int)(vector3.z * 30f));
        }
    }
}