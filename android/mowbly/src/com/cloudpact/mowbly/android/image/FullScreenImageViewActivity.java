package com.cloudpact.mowbly.android.image;

import java.util.ArrayList;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.view.ViewPager;

import com.cloudpact.mowbly.R;
/**
 * Image Gallery Activity
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class FullScreenImageViewActivity extends Activity{

	private FullScreenImageAdapter adapter;
	private ViewPager viewPager;

	@Override
	@SuppressWarnings("unchecked")
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		setContentView(R.layout.activity_fullscreen_view);

		viewPager = (ViewPager) findViewById(R.id.pager);
		Bundle extras = getIntent().getExtras();
		
		/* data
		 {
		 	"title": "",
		 	"images": [{
		 		"url": "",
		 		"title": ""
		 	}],
		 	"buttonLabel": "",
		 	"index": "",
		 	"loadingMsg": "",
		 	"errorMsg": ""
		 }
		 */

		String title = extras.getString("title");
		String buttonLabel = extras.getString("buttonLabel");
		int index = extras.getInt("index");
		String placeholder = extras.getString("placeholder");
		String loadingMsg = extras.getString("loadingMsg");
		String errorMsg = extras.getString("errorMsg");
		
		ArrayList<String> images = (ArrayList<String>) extras.get("images");
		ArrayList<String> titles = (ArrayList<String>) extras.get("titles");
		
		adapter = new FullScreenImageAdapter(FullScreenImageViewActivity.this,
				images, titles);
		adapter.setTitle(title);
		adapter.setButtonLabel(buttonLabel);
		adapter.setLoadingMessage(loadingMsg);
		adapter.setErrorMessage(errorMsg);
		adapter.setPlaceholderImage(placeholder);
		viewPager.setAdapter(adapter);

		// displaying selected image first
		viewPager.setCurrentItem(index);
	}
}