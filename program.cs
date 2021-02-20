using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;

static class Constants {
    public const int X = 25;
    public const int Y = 20;
}

/*
    This program will create and run a Windows Form application
    that can run a port scan.
*/
public class Program {

    private static Form f;
    private static Label portsLabel;
    private static TextBox singleIpTextBox;
    private static TextBox ipSubnetTextBox;
    private static TextBox portsTextBox;
    private static TextBox selectedIpTb;
    private static TextBox displayTextBox;
    private static GroupBox ipGroupBox;
    private static GroupBox portGroupBox;
    private static GroupBox displayGroupBox;
    private static RadioButton subnetRb;
    private static RadioButton ipRb;
    private static RadioButton selectedIpRb;
    private static RadioButton tcpRb;
    private static RadioButton udpRb;
    private static RadioButton bothRb;
    private static RadioButton selectedProtoRb;
    private static Button scanButton;
    private static ProgressBar pBar;

    static Program() {
        f = new Form();
        f.Text = "Port Scanner";
    }

    /*
        Creates the GroupBox for IPv4 address selection
    */
    static private void createIpGroupBox() {
        ipGroupBox = new GroupBox();
        subnetRb = new RadioButton();
        ipRb = new RadioButton();
        singleIpTextBox = new TextBox();
        ipSubnetTextBox = new TextBox();

        ipGroupBox.Controls.Add(subnetRb);
        ipGroupBox.Controls.Add(ipRb);
        ipGroupBox.Controls.Add(singleIpTextBox);
        ipGroupBox.Controls.Add(ipSubnetTextBox);

        ipRb.Location = new System.Drawing.Point(Constants.X, Constants.Y);
        ipRb.Name = "single_ip_radio_btn";
        //ipRb.Size = new System.Drawing.Size(67, 17);
        ipRb.AutoSize = true;
        ipRb.Text = "Address";
        ipRb.CheckedChanged += new EventHandler(ipRadioBtn_CheckedChanged);

        singleIpTextBox.Location = new Point(Constants.X + ipRb.Bounds.Right-20, Constants.Y);
        singleIpTextBox.Text = "ex: \"8.8.8.8\"";
        singleIpTextBox.Size = new Size(150,10);

        subnetRb.Location = new Point(Constants.X, Constants.Y + ipRb.Bounds.Bottom);
        subnetRb.Name = "subnet_radio_bn";
        //subnetRb.Size = new System.Drawing.Size(67, 17);
        subnetRb.AutoSize = true;
        subnetRb.Text = "Subnet";
        subnetRb.CheckedChanged += new EventHandler(ipRadioBtn_CheckedChanged);

        ipSubnetTextBox.Location = new Point(Constants.X + subnetRb.Bounds.Right-20, subnetRb.Location.Y);
        ipSubnetTextBox.Size = new Size(150, 10);
        ipSubnetTextBox.Text = "ex: \"192.168.1.0/24\"";

        //ipGroupBox.Size = new System.Drawing.Size(350, 200);
        ipGroupBox.Dock = (DockStyle.Top);
        ipGroupBox.Padding = new Padding(15);
        ipGroupBox.AutoSize = true;
        ipGroupBox.Text = "IP Addresses";
    }

    /*
        Creates the GroupBox for port and protocol selection
    */
    static private void createPortGroupBox() {
        portGroupBox = new GroupBox();
        tcpRb = new RadioButton();
        udpRb = new RadioButton();
        bothRb = new RadioButton();
        portsLabel = new Label();
        portsTextBox = new TextBox();

        portGroupBox.Controls.Add(tcpRb);
        portGroupBox.Controls.Add(udpRb);
        portGroupBox.Controls.Add(bothRb);
        portGroupBox.Controls.Add(portsLabel);
        portGroupBox.Controls.Add(portsTextBox);

        portsLabel.Location = new Point(Constants.X, Constants.Y);
        portsLabel.Text = "Ports";
        portsLabel.AutoSize = true;

        portsTextBox.Location = new Point(Constants.X + portsLabel.Bounds.Right+12, Constants.Y);
        portsTextBox.Text = "ex: \"80\", \"7,139,445\", \"1-1000\"";
        portsTextBox.Size = new Size(190,10);

        tcpRb.Location = new Point(Constants.X, portsLabel.Location.Y + 20);
        tcpRb.Name = "tcp_radio_btn";
        tcpRb.AutoSize = true;
        tcpRb.Text = "tcp";
        tcpRb.CheckedChanged += new EventHandler(protocolRadioBtn_CheckedChanged);

        udpRb.Location = new Point(Constants.X, tcpRb.Location.Y + 20);
        udpRb.Name = "udp_radio_btn";
        udpRb.AutoSize = true;
        udpRb.Text = "udp";
        udpRb.CheckedChanged += new EventHandler(protocolRadioBtn_CheckedChanged);

        bothRb.Location = new Point(Constants.X, udpRb.Location.Y + 20);
        bothRb.Name = "both_radio_btn";
        bothRb.AutoSize = true;
        bothRb.Text = "both";
        bothRb.CheckedChanged += new EventHandler(protocolRadioBtn_CheckedChanged);

        portGroupBox.Dock = (DockStyle.Top);
        portGroupBox.Padding = new Padding(15);
        portGroupBox.AutoSize = true;
        portGroupBox.Text = "Ports";
    }

    /*
        Creates the Button for user to click when ready to scan
    */
    static private void createScanButton() {
        scanButton = new Button();
        //scanButton.Location = new Point(Constants.X + 175, portsTextBox.Location.Y + 50);
        scanButton.Dock = (DockStyle.Top);
        scanButton.AutoSize = true;
        scanButton.Text = "Scan";
        scanButton.Enabled = false;
        scanButton.Click += new EventHandler(scanButton_Click);
    }

    /*
        Creates the GroupBox with the TextBox that displays scan results
    */
    static private void createDisplayGroupBox() {
        displayGroupBox = new GroupBox();
        displayTextBox = new TextBox();

        displayGroupBox.Controls.Add(displayTextBox);

        displayTextBox.Dock = DockStyle.Fill;
        displayTextBox.Multiline = true;
        displayTextBox.ScrollBars = ScrollBars.Vertical;
        //displayTextBox.Enabled = false;

        displayGroupBox.Dock = DockStyle.Top;
        displayGroupBox.Padding = new Padding(15);
        displayGroupBox.AutoSize = true;
        displayGroupBox.Text = "Results";
    }

    /*
        Creates the ProgressBar object
    */
    static private void createProgressBar() {
        pBar = new ProgressBar();
        pBar.Visible = true;
        pBar.Value = 0;
        pBar.Minimum = 0;
        pBar.Step = 1;
        pBar.Dock = DockStyle.Top;
    }

    /*
        Saves which radio button for the IP address selection was selected
    */
    static void ipRadioBtn_CheckedChanged(object sender, EventArgs e) {
        RadioButton rb = sender as RadioButton;

        if (rb == null) {
            MessageBox.Show("Sender is not a RadioButton");
            return;
        }

        if (rb.Checked) {
            selectedIpRb = rb;
            if (rb == ipRb) {
                selectedIpTb = singleIpTextBox;
            }
            else {
                selectedIpTb = ipSubnetTextBox;
            }
            updateButton();
        }
    }

    /*
        Saves which radio button for the protocol selection was selected
    */
    static void protocolRadioBtn_CheckedChanged(object sender, EventArgs e) {
        RadioButton rb = sender as RadioButton;

        if (rb == null) {
            MessageBox.Show("Sender is not a RadioButton");
            return;
        }

        if (rb.Checked) {
            selectedProtoRb = rb;
            updateButton();
        }
    }

    /*
        Enables/disables the scan button
    */
    private static void updateButton() {
        scanButton.Enabled = (singleIpTextBox.Text != string.Empty || ipSubnetTextBox.Text != string.Empty)
        && (ipRb.Checked || subnetRb.Checked) && (portsTextBox.Text != string.Empty) && (tcpRb.Checked || udpRb.Checked || bothRb.Checked);
    }

    /*
        Calls methods in the nested PortScanner class when the scan button is clicked
    */
    static void scanButton_Click(object sender, EventArgs e) {
        //MessageBox.Show(selectedIpRb.Text);
        displayTextBox.Text = "";

        if (selectedIpRb == ipRb) {
            PortScanner.scanSingleIp(selectedIpTb.Text, portsTextBox.Text, selectedProtoRb.Text);
        }
        else {
            PortScanner.scanIpSubnet(selectedIpTb.Text, portsTextBox.Text, selectedProtoRb.Text);
        }
    }

    [STAThread]
    public static void Main() {
        createIpGroupBox();
        createPortGroupBox();
        createScanButton();
        createDisplayGroupBox();
        createProgressBar();

        //f.ClientSize = new System.Drawing.Size(400, 400);
        f.AutoSize = true;
        f.Controls.Add(displayGroupBox);
        f.Controls.Add(pBar);
        f.Controls.Add(scanButton);
        f.Controls.Add(portGroupBox);
        f.Controls.Add(ipGroupBox);
        Application.Run(f);
    }

    /*
        Handles port scanning for the Program class whenever the scan button is clicked
    */
    static class PortScanner {
        /*
            Scans an IPv4 address like 192.168.1.209
        */
        static public void scanSingleIp(string ipaddr, string portTbText, string protocol) {
            // Parse port numbers
            string[] portStrs;
            int[] portInts;
            if (portTbText.Contains(",")) {
                portStrs = portTbText.Split(',');
                portInts = new int[portStrs.Length];
                for (int i = 0; i < portStrs.Length; i++) {
                    portInts[i] = Convert.ToInt32(portStrs[i]);
                }
            }
            else if (portTbText.Contains("-")) {
                portStrs = portTbText.Split('-');
                int start = Convert.ToInt32(portStrs[0]);
                int end = Convert.ToInt32(portStrs[1]);
                portInts = new int[end - start + 1];
                for (int i = start; i <= end; i++) {
                    portInts[i - start] = i;
                }
            }
            else {
                portInts = new int[1];
                portInts[0] = Convert.ToInt32(portTbText);
            }

            // Verifying portInts correctness
            // for (int i = 0; i < portInts.Length; i++) {
            //     System.Console.WriteLine(portInts[i]);
            // }

            pBar.Value = 0;
            pBar.Maximum = protocol == "both" ? 2*portInts.Length : portInts.Length;

            if (protocol == "tcp" || protocol == "both") {
                //Scan ports from int[] portInts on 192.168.1.209
                foreach (int port in portInts) {
                    using (TcpClient tcpClient = new TcpClient()) {
                        IAsyncResult ar = tcpClient.BeginConnect(ipaddr, port, null, null);
                        System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                        try {
                            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false)) {
                                tcpClient.Close();
                                throw new TimeoutException();
                            }
                            //tcpClient.ConnectAsync(ipaddr, port);
                            //System.Console.WriteLine(port + "/tcp open");

                            tcpClient.EndConnect(ar);
                            displayTextBox.Text += port;
                            displayTextBox.Text += "/tcp open\n";
                        } catch {
                            //System.Console.WriteLine(port + "/tcp closed");
                            displayTextBox.Text += port;
                            displayTextBox.Text += "/tcp closed\n";
                        } finally {
                            if (wh == null) {
                                System.Console.WriteLine("wh is null");
                            }
                            wh.Close();
                        }
                        pBar.PerformStep();
                    }
                }
            }

            if (protocol == "udp" || protocol == "both") {
                //Scan ports from int[] portInts
                foreach (int port in portInts) {
                    UdpClient udpClient = new UdpClient();
                    try {
                        udpClient.Connect(ipaddr, port);
                        //System.Console.WriteLine(port + "/udp open");
                        displayTextBox.Text += port;
                        displayTextBox.Text += "/udp open\n";
                    } catch {
                        //System.Console.WriteLine(port + "/udp closed");
                        displayTextBox.Text += port;
                        displayTextBox.Text += "/udp closed\n";
                    }
                    udpClient.Close();
                    pBar.PerformStep();
                }
            }


        }

        /*
            Scans a subnet like 192.168.1.0/24 for the specified port(s) and protocol(s)
        */
        static public void scanIpSubnet(string subnet, string portTbText, string protocol) {
            string[] substrs = subnet.Split('/');
            string ipsubnet = substrs[0];
            int numMaskBits = Convert.ToInt16(substrs[1]);
            string[] byteStrs = ipsubnet.Split('.');

            byte[] bytes = new byte[4];

            for(int i = 0; i < byteStrs.Length; i++) {
                bytes[i] = Convert.ToByte(byteStrs[i]);
            }

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            int maskint = BitConverter.ToInt32(bytes, 0);
            maskint = maskint >> (32 - numMaskBits);
            maskint = maskint << (32 - numMaskBits);

            int max = Convert.ToInt32(Math.Pow(2, (32-numMaskBits)));
            int[] ipaddrs = new int[max - 1];

            for (int i = 1; i < max - 1; i++) {
                ipaddrs[i - 1] = maskint | i;

                byte[] tmpbytes = BitConverter.GetBytes(ipaddrs[i - 1]);
                if (BitConverter.IsLittleEndian) {
                    Array.Reverse(tmpbytes);
                }
                // Console.WriteLine("{0}.{1}.{2}.{3}", tmpbytes[0], tmpbytes[1], tmpbytes[2], tmpbytes[3]);
                string ipToScan = "";
                for (int j = 0; j < tmpbytes.Length; j++) {
                    ipToScan += Convert.ToString(Convert.ToInt16(tmpbytes[j]));
                    if (j < tmpbytes.Length - 1) {
                        ipToScan += '.';
                    }
                }
                //Console.WriteLine(ipToScan);

                scanSingleIp(ipToScan, portTbText, protocol);
            }
        }
    }
}