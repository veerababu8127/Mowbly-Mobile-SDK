package com.cloudpact.mowbly.android.feature;

import java.util.ArrayList;

import android.content.Intent;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.Intents;
import com.cloudpact.mowbly.android.image.FullScreenImageViewActivity;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the image gallery feature
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>, Sathish Kumar <sathish@cloudpact.com>
 * @since 2.0.0
 */

public class ImageGalleryFeature extends BaseFeature {

	/** Exposed name of the MessageFeature */
    public static final String NAME = "imagegallery";
    protected static final Logger logger = Logger.getLogger();
    protected static final String TAG = "ImageGallery";
	
	public ImageGalleryFeature() {
		super(NAME);
	}

	/**
	 * Opens Image Gallery with options containing title, button label, current index and the array of images to be shown
	 * @param options
	 * @return Response
	 */
	@Method(async = true, args = {
		@Argument(name = "options", type = JsonObject.class)
	})
	 public Response showAsGallery(JsonObject options) {
		// Launch the gallery activity
		logger.info(TAG, "Opening image gallery");
		Intent i = new Intent(((FeatureBinder)this.binder).getActivity().getApplicationContext(), FullScreenImageViewActivity.class);
		String title = options.get("title").getAsString();
		String buttonLabel = options.get("buttonLabel").getAsString();
		int index = options.get("index").getAsInt();
		String loadingMsg = options.get("loadingMsg").getAsString();
		String errorMsg = options.get("errorMsg").getAsString();
		
		i.putExtra("title", title);
		i.putExtra("buttonLabel", buttonLabel);
		i.putExtra("index", index);
		i.putExtra("loadingMsg", loadingMsg);
		i.putExtra("errorMsg", errorMsg);
		 
		ArrayList<String> images = new ArrayList<String>();
		ArrayList<String> titles = new ArrayList<String>();
		JsonArray aImages = options.get("images").getAsJsonArray();
		for(JsonElement e:aImages) {
			JsonObject img = e.getAsJsonObject();
			images.add(img.get("url").getAsString());
			titles.add(img.get("title").getAsString());
		}
		JsonElement jPlaceHolder = options.get("placeholder");
		if(jPlaceHolder != null) {
			String placeholder = jPlaceHolder.getAsString();
			i.putExtra("placeholder", placeholder);
		}
		i.putExtra("images", images);
		i.putExtra("titles", titles);
		// Start the activity
		final PageActivity activity = ((FeatureBinder)this.binder).getActivity();
		activity.startActivityForResult(i, Intents.getRequestCode(activity, R.string.action_ACTIVITY_IMAGE_GALLERY));
		logger.info(TAG, "Started image gallery");
		return null;
	 }
}