//*******************************************************************
/*
 *	Project:	Alfray.BatteryInfo
 * 
 *	File:		BatteryStatus.cs
 * 
 *	RM (c) 2003
 * 
 */
//*******************************************************************

using System;

// -----------------------

using System.Runtime.InteropServices;

// -----------------------

//*************************************
namespace Alfray.BatteryInfo
{


	//****************************************************
	/// <summary>
	/// Summary description for BatteryStatus.
	/// </summary>
	public class BatteryStatus
	{
		// ------- public properties ------

		//********************
		public int LifePercent
		{
			get
			{
				return mPercent;
			}
		}

		//**********************
		public TimeSpan TimeLeft
		{
			get
			{
				return mTimeLeft;
			}
		}

		//***********************
		public TimeSpan TimeTotal
		{
			get
			{
				return mTimeTotal;
			}
		}


		//******************
		public string AcLine
		{
			get
			{
				return mAcLine;
			}
		}


		//****************
		public string Flag
		{
			get
			{
				return mFlag;
			}
		}

		// ------- public methods --------


		//***********************************
		[StructLayout(LayoutKind.Sequential)]
		public struct SysPowerStatus
		{
			public Byte ACLineStatus;			// 0=Offline, 1=Online, 255=Unknown
			public Byte BatteryFlag;			// 1=High, 2=Low, 4=Critical, 8=Charging, 128=No system battery, 255=Unknown
			public Byte BatteryLifePercent;	// 0..100 or 255=Unknown
			public Byte Reserved1;				// 0
			public Int32 BatteryLifeTime;		// seconds, -1=Unknown
			public Int32 BatteryFullLifeTime;	// seconds, -1=Unknown
		}

		//***********************************
		[DllImport("KERNEL32.DLL",
			 EntryPoint="GetSystemPowerStatus",
			 SetLastError=true,
			 CharSet=CharSet.Unicode,
			 ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		public static extern bool GetSystemPowerStatus(ref SysPowerStatus status);

		
		//********************************
		public BatteryStatus()
		{
			UpdateStatus();
		}


		//************************
		public bool UpdateStatus()
		{
			SysPowerStatus st = new SysPowerStatus();
			bool ok = GetSystemPowerStatus(ref st);

			if (!ok)
				return false;

			this.mPercent = st.BatteryLifePercent;

			switch(st.ACLineStatus)
			{
				case 0:
					this.mAcLine = "Offline";
					break;
				case 1:
					if (st.BatteryFlag == 255 || (st.BatteryFlag & 8) == 0)
						this.mAcLine = "Online";
					break;
				case 255:
					this.mAcLine = "Unknown";
					break;
				default:
					this.mAcLine = String.Format("{0} (Unexpected)", st.ACLineStatus);
					break;
			}

			if (st.BatteryFlag == 255)
			{
				mFlag = "Unknown";
			}
			else if (st.BatteryFlag == 0)
			{
				mFlag = "";
			}
			else
			{
				mFlag = "";
				if ((st.BatteryFlag &   1) != 0) mFlag += ", High";
				if ((st.BatteryFlag &   2) != 0) mFlag += ", Low";
				if ((st.BatteryFlag &   4) != 0) mFlag += ", Critical";
				if ((st.BatteryFlag &   8) != 0) mFlag += ", Charging";
				if ((st.BatteryFlag & 128) != 0) mFlag += ", No system battery";
				this.mFlag = mFlag.Substring(2);
			}

			// if charging...
			bool is_charging = ((st.BatteryFlag & 8) != 0);
			if (is_charging)
			{
				long new_tick = DateTime.Now.Ticks;

				if (mLastPercentUpdate == -1)
				{
					mLastPercentUpdate = new_tick;
					mLastPercentValue = mPercent;
					mChargeTime = new TimeSpan(0);
				}
				else if (mPercent > mLastPercentValue && mPercent < 100)
				{
					int pdelta = mPercent - mLastPercentValue;
					mChargeTime = new TimeSpan((100 - mPercent) * (new_tick - mLastPercentUpdate) / pdelta);

					mLastPercentUpdate = new_tick;
					mLastPercentValue = mPercent;
				}
			}
			else
			{
				mLastPercentUpdate = -1;
			}

			if (st.BatteryLifeTime > 0)
				this.mTimeLeft = new TimeSpan(TimeSpan.TicksPerSecond * st.BatteryLifeTime);
			else if (is_charging)
				this.mTimeLeft = mChargeTime;
			else
				this.mTimeLeft = new TimeSpan(0);

			if (st.BatteryFullLifeTime > 0)
				this.mTimeTotal = new TimeSpan(TimeSpan.TicksPerSecond * st.BatteryFullLifeTime);
			else if (is_charging)
				this.mTimeTotal = mChargeTime;
			else
				this.mTimeTotal = new TimeSpan(0);

			return true;
		}

		// ------- private methods ------- 

		// ------- private properties ------- 

		int			mPercent	= 0;
		TimeSpan	mTimeLeft	= new TimeSpan(0);
		TimeSpan	mTimeTotal	= new TimeSpan(0);
		string		mAcLine;
		string		mFlag;

		long		mLastPercentUpdate	= -1;
		int			mLastPercentValue	= -1;
		TimeSpan	mChargeTime			= new TimeSpan(0);

	} // class BatteryStatus
} // namespace Alfray.BatteryInfo

//*******************************************************************
/*
 *	$Log: BatteryStatus.cs,v $
 *	Revision 1.2  2004-06-16 03:13:27  ralf
 *	Charging time indicator
 *
 *	Revision 1.1.1.1  2003/08/01 00:49:55  ralf
 *	Initial working version
 *	
 * 
 */
//*******************************************************************

