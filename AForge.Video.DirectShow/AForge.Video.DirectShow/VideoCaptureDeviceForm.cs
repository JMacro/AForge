using AForge.Video.DirectShow.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Video.DirectShow
{
	public class VideoCaptureDeviceForm : Form
	{
		private IContainer components;

		private Button cancelButton;

		private Button okButton;

		private ComboBox devicesCombo;

		private GroupBox groupBox1;

		private PictureBox pictureBox;

		private Label label1;

		private Label snapshotsLabel;

		private ComboBox snapshotResolutionsCombo;

		private ComboBox videoResolutionsCombo;

		private Label label2;

		private ComboBox videoInputsCombo;

		private Label label3;

		private FilterInfoCollection videoDevices;

		private VideoCaptureDevice videoDevice;

		private Dictionary<string, VideoCapabilities> videoCapabilitiesDictionary = new Dictionary<string, VideoCapabilities>();

		private Dictionary<string, VideoCapabilities> snapshotCapabilitiesDictionary = new Dictionary<string, VideoCapabilities>();

		private VideoInput[] availableVideoInputs;

		private bool configureSnapshots;

		private string videoDeviceMoniker = string.Empty;

		private Size captureSize = new Size(0, 0);

		private Size snapshotSize = new Size(0, 0);

		private VideoInput videoInput = VideoInput.Default;

		public bool ConfigureSnapshots
		{
			get
			{
				return configureSnapshots;
			}
			set
			{
				configureSnapshots = value;
				snapshotsLabel.Visible = value;
				snapshotResolutionsCombo.Visible = value;
			}
		}

		public VideoCaptureDevice VideoDevice => videoDevice;

		public string VideoDeviceMoniker
		{
			get
			{
				return videoDeviceMoniker;
			}
			set
			{
				videoDeviceMoniker = value;
			}
		}

		public Size CaptureSize
		{
			get
			{
				return captureSize;
			}
			set
			{
				captureSize = value;
			}
		}

		public Size SnapshotSize
		{
			get
			{
				return snapshotSize;
			}
			set
			{
				snapshotSize = value;
			}
		}

		public VideoInput VideoInput
		{
			get
			{
				return videoInput;
			}
			set
			{
				videoInput = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			cancelButton = new System.Windows.Forms.Button();
			okButton = new System.Windows.Forms.Button();
			devicesCombo = new System.Windows.Forms.ComboBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			videoInputsCombo = new System.Windows.Forms.ComboBox();
			label3 = new System.Windows.Forms.Label();
			snapshotsLabel = new System.Windows.Forms.Label();
			snapshotResolutionsCombo = new System.Windows.Forms.ComboBox();
			videoResolutionsCombo = new System.Windows.Forms.ComboBox();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			pictureBox = new System.Windows.Forms.PictureBox();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
			SuspendLayout();
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			cancelButton.Location = new System.Drawing.Point(239, 190);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.TabIndex = 11;
			cancelButton.Text = "Cancel";
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			okButton.Location = new System.Drawing.Point(149, 190);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(75, 23);
			okButton.TabIndex = 10;
			okButton.Text = "OK";
			okButton.Click += new System.EventHandler(okButton_Click);
			devicesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			devicesCombo.FormattingEnabled = true;
			devicesCombo.Location = new System.Drawing.Point(100, 40);
			devicesCombo.Name = "devicesCombo";
			devicesCombo.Size = new System.Drawing.Size(325, 21);
			devicesCombo.TabIndex = 9;
			devicesCombo.SelectedIndexChanged += new System.EventHandler(devicesCombo_SelectedIndexChanged);
			groupBox1.Controls.Add(videoInputsCombo);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(snapshotsLabel);
			groupBox1.Controls.Add(snapshotResolutionsCombo);
			groupBox1.Controls.Add(videoResolutionsCombo);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(pictureBox);
			groupBox1.Controls.Add(devicesCombo);
			groupBox1.Location = new System.Drawing.Point(10, 10);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(440, 165);
			groupBox1.TabIndex = 12;
			groupBox1.TabStop = false;
			groupBox1.Text = "Video capture device settings";
			videoInputsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			videoInputsCombo.FormattingEnabled = true;
			videoInputsCombo.Location = new System.Drawing.Point(100, 130);
			videoInputsCombo.Name = "videoInputsCombo";
			videoInputsCombo.Size = new System.Drawing.Size(150, 21);
			videoInputsCombo.TabIndex = 17;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(100, 115);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(63, 13);
			label3.TabIndex = 16;
			label3.Text = "Video input:";
			snapshotsLabel.AutoSize = true;
			snapshotsLabel.Location = new System.Drawing.Point(275, 70);
			snapshotsLabel.Name = "snapshotsLabel";
			snapshotsLabel.Size = new System.Drawing.Size(101, 13);
			snapshotsLabel.TabIndex = 15;
			snapshotsLabel.Text = "Snapshot resoluton:";
			snapshotResolutionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			snapshotResolutionsCombo.FormattingEnabled = true;
			snapshotResolutionsCombo.Location = new System.Drawing.Point(275, 85);
			snapshotResolutionsCombo.Name = "snapshotResolutionsCombo";
			snapshotResolutionsCombo.Size = new System.Drawing.Size(150, 21);
			snapshotResolutionsCombo.TabIndex = 14;
			videoResolutionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			videoResolutionsCombo.FormattingEnabled = true;
			videoResolutionsCombo.Location = new System.Drawing.Point(100, 85);
			videoResolutionsCombo.Name = "videoResolutionsCombo";
			videoResolutionsCombo.Size = new System.Drawing.Size(150, 21);
			videoResolutionsCombo.TabIndex = 13;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(100, 70);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(83, 13);
			label2.TabIndex = 12;
			label2.Text = "Video resoluton:";
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(100, 25);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(72, 13);
			label1.TabIndex = 11;
			label1.Text = "Video device:";
			pictureBox.Image = AForge.Video.DirectShow.Properties.Resources.camera;
			pictureBox.Location = new System.Drawing.Point(20, 28);
			pictureBox.Name = "pictureBox";
			pictureBox.Size = new System.Drawing.Size(64, 64);
			pictureBox.TabIndex = 10;
			pictureBox.TabStop = false;
			base.AcceptButton = okButton;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = cancelButton;
			base.ClientSize = new System.Drawing.Size(462, 221);
			base.Controls.Add(groupBox1);
			base.Controls.Add(cancelButton);
			base.Controls.Add(okButton);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "VideoCaptureDeviceForm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Open local  video capture device";
			base.Load += new System.EventHandler(VideoCaptureDeviceForm_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
			ResumeLayout(false);
		}

		public VideoCaptureDeviceForm()
		{
			InitializeComponent();
			ConfigureSnapshots = false;
			try
			{
				videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
				if (videoDevices.Count == 0)
				{
					throw new ApplicationException();
				}
				foreach (FilterInfo videoDevice2 in videoDevices)
				{
					devicesCombo.Items.Add(videoDevice2.Name);
				}
			}
			catch (ApplicationException)
			{
				devicesCombo.Items.Add("No local capture devices");
				devicesCombo.Enabled = false;
				okButton.Enabled = false;
			}
		}

		private void VideoCaptureDeviceForm_Load(object sender, EventArgs e)
		{
			int selectedIndex = 0;
			for (int i = 0; i < videoDevices.Count; i++)
			{
				if (videoDeviceMoniker == videoDevices[i].MonikerString)
				{
					selectedIndex = i;
					break;
				}
			}
			devicesCombo.SelectedIndex = selectedIndex;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			videoDeviceMoniker = videoDevice.Source;
			if (videoCapabilitiesDictionary.Count != 0)
			{
				VideoCapabilities videoCapabilities = videoCapabilitiesDictionary[(string)videoResolutionsCombo.SelectedItem];
				videoDevice.VideoResolution = videoCapabilities;
				captureSize = videoCapabilities.FrameSize;
			}
			if (configureSnapshots && snapshotCapabilitiesDictionary.Count != 0)
			{
				VideoCapabilities videoCapabilities2 = snapshotCapabilitiesDictionary[(string)snapshotResolutionsCombo.SelectedItem];
				videoDevice.ProvideSnapshots = true;
				videoDevice.SnapshotResolution = videoCapabilities2;
				snapshotSize = videoCapabilities2.FrameSize;
			}
			if (availableVideoInputs.Length != 0)
			{
				videoInput = availableVideoInputs[videoInputsCombo.SelectedIndex];
				videoDevice.CrossbarVideoInput = videoInput;
			}
		}

		private void devicesCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (videoDevices.Count != 0)
			{
				videoDevice = new VideoCaptureDevice(videoDevices[devicesCombo.SelectedIndex].MonikerString);
				EnumeratedSupportedFrameSizes(videoDevice);
			}
		}

		private void EnumeratedSupportedFrameSizes(VideoCaptureDevice videoDevice)
		{
			Cursor = Cursors.WaitCursor;
			videoResolutionsCombo.Items.Clear();
			snapshotResolutionsCombo.Items.Clear();
			videoInputsCombo.Items.Clear();
			videoCapabilitiesDictionary.Clear();
			snapshotCapabilitiesDictionary.Clear();
			try
			{
				VideoCapabilities[] videoCapabilities = videoDevice.VideoCapabilities;
				int selectedIndex = 0;
				VideoCapabilities[] array = videoCapabilities;
				foreach (VideoCapabilities videoCapabilities2 in array)
				{
					string text = $"{videoCapabilities2.FrameSize.Width} x {videoCapabilities2.FrameSize.Height}";
					if (!videoResolutionsCombo.Items.Contains(text))
					{
						if (captureSize == videoCapabilities2.FrameSize)
						{
							selectedIndex = videoResolutionsCombo.Items.Count;
						}
						videoResolutionsCombo.Items.Add(text);
					}
					if (!videoCapabilitiesDictionary.ContainsKey(text))
					{
						videoCapabilitiesDictionary.Add(text, videoCapabilities2);
					}
				}
				if (videoCapabilities.Length == 0)
				{
					videoResolutionsCombo.Items.Add("Not supported");
				}
				videoResolutionsCombo.SelectedIndex = selectedIndex;
				if (configureSnapshots)
				{
					VideoCapabilities[] snapshotCapabilities = videoDevice.SnapshotCapabilities;
					int selectedIndex2 = 0;
					VideoCapabilities[] array2 = snapshotCapabilities;
					foreach (VideoCapabilities videoCapabilities3 in array2)
					{
						string text2 = $"{videoCapabilities3.FrameSize.Width} x {videoCapabilities3.FrameSize.Height}";
						if (!snapshotResolutionsCombo.Items.Contains(text2))
						{
							if (snapshotSize == videoCapabilities3.FrameSize)
							{
								selectedIndex2 = snapshotResolutionsCombo.Items.Count;
							}
							snapshotResolutionsCombo.Items.Add(text2);
							snapshotCapabilitiesDictionary.Add(text2, videoCapabilities3);
						}
					}
					if (snapshotCapabilities.Length == 0)
					{
						snapshotResolutionsCombo.Items.Add("Not supported");
					}
					snapshotResolutionsCombo.SelectedIndex = selectedIndex2;
				}
				availableVideoInputs = videoDevice.AvailableCrossbarVideoInputs;
				int selectedIndex3 = 0;
				VideoInput[] array3 = availableVideoInputs;
				foreach (VideoInput videoInput in array3)
				{
					string item = $"{videoInput.Index}: {videoInput.Type}";
					if (videoInput.Index == this.videoInput.Index && videoInput.Type == this.videoInput.Type)
					{
						selectedIndex3 = videoInputsCombo.Items.Count;
					}
					videoInputsCombo.Items.Add(item);
				}
				if (availableVideoInputs.Length == 0)
				{
					videoInputsCombo.Items.Add("Not supported");
				}
				videoInputsCombo.SelectedIndex = selectedIndex3;
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}
	}
}
