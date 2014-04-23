package com.cloudpact.mowbly.android.ui;

import android.app.Activity;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.cloudpact.mowbly.R;

/**
 * SplashActivity
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public abstract class SplashActivity extends Activity {

    protected ProgressBar progressBar;

    protected TextView progressText;

    protected InstallationTask installationTask;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        getWindow().requestFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.activity_splash);

        progressBar = (ProgressBar) findViewById(R.id.installProgressBar);
        progressBar.setIndeterminate(true);
        progressText = (TextView) findViewById(R.id.installProgressText);

        installationTask = createInstallationTask();
    }

    @Override
    protected void onResume() {
        super.onResume();

        progressBar.setVisibility(View.VISIBLE);
        if (installationTask != null && !installationTask.isRunning()) {
            installationTask = createInstallationTask();
            installationTask.execute();
        }
    }

    @Override
    public void onBackPressed() {
        moveTaskToBack(true);
    }

    protected InstallationTask createInstallationTask() {
        return new InstallationTask();
    }

    protected void onInstallationFailed() {
        progressBar.setVisibility(View.INVISIBLE);
    }

    protected void onInstallationCompleted() {
        installationTask = null;

        setResult(Activity.RESULT_OK);
        finish();
    }

    protected class InstallationTask extends AsyncTask<Void, String, Boolean>{

        protected boolean isRunning;

        public InstallationTask() {
        }

        public boolean isRunning() {
            return isRunning;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            isRunning = true;
        }

        @Override
        protected Boolean doInBackground(Void... params) {
            return true;
        }

        @Override
        protected void onProgressUpdate(String... values) {
            progressText.setText(values[0]);
        }

        @Override
        protected void onPostExecute(Boolean result) {
            isRunning = false;
            if (!result) {
                onInstallationFailed();
            } else {
                onInstallationCompleted();
            }
        }
    }
}