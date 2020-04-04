using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Tooling.CrmConnectControl;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace CrmSvcUtil.InteractiveLogin
{
    public partial class CRMInteractiveLogin : Window, IComponentConnector
    {
        private CrmServiceClient CrmSvc;
        private bool bIsConnectedComplete;
        private CrmConnectionManager mgr;
        private bool resetUiFlag;

        public CrmConnectionManager CrmConnectionMgr
        {
            get
            {
                return this.mgr;
            }
        }

        public string HostApplicatioNameOveride { get; set; }

        public string HostProfileName { get; set; }

        public bool ForceDirectLogin { get; set; }

        public event EventHandler ConnectionToCrmCompleted;

        public CRMInteractiveLogin()
        {
            this.InitializeComponent();
            this.HostApplicatioNameOveride = (string)null;
            this.HostProfileName = "default";
            this.ForceDirectLogin = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.bIsConnectedComplete = false;
            this.ExecuteLoginProcess();
        }

        private void SetupLoginControl()
        {
            this.bIsConnectedComplete = false;
            this.mgr = new CrmConnectionManager();
            this.mgr.ClientId = "2ad88395-b77d-4561-9441-d0e40824f9bc";
            this.mgr.RedirectUri = new Uri("app://5d3e90d6-aa8e-48a8-8f2c-58b45cc67315");
            this.mgr.ParentControl = (UserControl)this.CrmLoginCtrl;
            this.mgr.UseUserLocalDirectoryForConfigStore = true;
            this.mgr.HostApplicatioNameOveride = this.HostApplicatioNameOveride;
            this.mgr.ProfileName = this.HostProfileName;
            this.CrmLoginCtrl.SetGlobalStoreAccess(this.mgr);
            this.CrmLoginCtrl.SetControlMode(ServerLoginConfigCtrlMode.FullLoginPanel);
            this.CrmLoginCtrl.ConnectionCheckBegining += new EventHandler(this.CrmLoginCtrl_ConnectionCheckBegining);
            this.CrmLoginCtrl.ConnectErrorEvent += new EventHandler<ConnectErrorEventArgs>(this.CrmLoginCtrl_ConnectErrorEvent);
            this.CrmLoginCtrl.ConnectionStatusEvent += new EventHandler<ConnectStatusEventArgs>(this.CrmLoginCtrl_ConnectionStatusEvent);
            this.CrmLoginCtrl.UserCancelClicked += new EventHandler(this.CrmLoginCtrl_UserCancelClicked);
        }

        public void ExecuteLoginProcess()
        {
            if (this.mgr == null)
                this.SetupLoginControl();
            if (this.ForceDirectLogin || this.mgr.RequireUserLogin())
                return;
            this.CrmLoginCtrl.IsEnabled = false;
            this.mgr.ServerConnectionStatusUpdate += new EventHandler<ServerConnectStatusEventArgs>(this.mgr_ServerConnectionStatusUpdate);
            this.mgr.ConnectionCheckComplete += new EventHandler<ServerConnectStatusEventArgs>(this.mgr_ConnectionCheckComplete);
            this.mgr.ConnectToServerCheck();
            this.CrmLoginCtrl.ShowMessageGrid();
        }

        private void mgr_ServerConnectionStatusUpdate(object sender, ServerConnectStatusEventArgs e)
        {
            this.Dispatcher.Invoke(() => { this.Title = string.IsNullOrWhiteSpace(e.StatusMessage) ? e.ErrorMessage : e.StatusMessage; }, DispatcherPriority.Normal);
        }

        private void mgr_ConnectionCheckComplete(object sender, ServerConnectStatusEventArgs e)
        {
            ((CrmConnectionManager)sender).ConnectionCheckComplete -= new EventHandler<ServerConnectStatusEventArgs>(this.mgr_ConnectionCheckComplete);
            ((CrmConnectionManager)sender).ServerConnectionStatusUpdate -= new EventHandler<ServerConnectStatusEventArgs>(this.mgr_ServerConnectionStatusUpdate);
            if (!e.Connected)
            {
                if (e.MultiOrgsFound)
                {
                    int num1 = (int)MessageBox.Show("Unable to Login to CRM using cached credentials. Org Not found", "Login Failure");
                }
                else
                {
                    int num2 = (int)MessageBox.Show("Unable to Login to CRM using cached credentials", "Login Failure");
                }
                this.resetUiFlag = true;
                this.CrmLoginCtrl.GoBackToLogin();
                this.Dispatcher.Invoke(() =>
                {
                    this.Title = "Failed to Login with cached credentials.";
                    int num = (int)MessageBox.Show(this.Title, "Notification from CRM ConnectionManager", MessageBoxButton.OK, MessageBoxImage.Hand);
                    this.CrmLoginCtrl.IsEnabled = true;
                }, DispatcherPriority.Normal);
                this.resetUiFlag = false;
            }
            else
            {
                if (!e.Connected || this.bIsConnectedComplete)
                    return;
                this.ProcessSuccess();
            }
        }

        private void CrmLoginCtrl_ConnectionCheckBegining(object sender, EventArgs e)
        {
            this.bIsConnectedComplete = false;
            this.Dispatcher.Invoke(() =>
            {
                this.Title = "Starting Login Process. ";
                this.CrmLoginCtrl.IsEnabled = true;
            }, DispatcherPriority.Normal);
        }

        private void CrmLoginCtrl_ConnectionStatusEvent(object sender, ConnectStatusEventArgs e)
        {
            if (!e.ConnectSucceeded || this.bIsConnectedComplete)
                return;
            this.ProcessSuccess();
        }

        private void CrmLoginCtrl_ConnectErrorEvent(object sender, ConnectErrorEventArgs e)
        {
        }

        private void CrmLoginCtrl_UserCancelClicked(object sender, EventArgs e)
        {
            if (this.resetUiFlag)
                return;
            this.Close();
        }

        private void ProcessSuccess()
        {
            this.resetUiFlag = true;
            this.bIsConnectedComplete = true;
            this.CrmSvc = this.mgr.CrmSvc;
            this.CrmLoginCtrl.GoBackToLogin();
            this.Dispatcher.Invoke(() =>
            {
                this.Title = "Login Complete.";
                this.CrmLoginCtrl.IsEnabled = true;
            }, DispatcherPriority.Normal);

            if (this.ConnectionToCrmCompleted != null)
                this.ConnectionToCrmCompleted((object)this, (EventArgs)null);

            this.resetUiFlag = false;
            this.Dispatcher.Invoke(() =>
            {
                this.DialogResult = new bool?(true);
                this.Close();
            }, DispatcherPriority.Normal);
        }
    }
}