package com.cloudpact.mowbly.android.ui;



/**
 * OpenSplashActivity
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class OpenSplashActivity extends SplashActivity {
	private static long SplashTime = 1000;
	
	@Override
	protected InstallationTask createInstallationTask() {
		return new OpenInstallationTask();
	}
	
	/**
	 * Show splash for specified SplashTime every time app is started
	 * 
	 * @author Aravind Baskaran <aravind@cloudpact.com>
	 */
	protected class OpenInstallationTask extends InstallationTask {
        public OpenInstallationTask() {
            super();
        }

        @Override
        protected Boolean doInBackground(Void... params) {
        	try {
				Thread.sleep(SplashTime);
			} catch (InterruptedException e) {
				// Log
			}
            return true;
        }
    }
}
