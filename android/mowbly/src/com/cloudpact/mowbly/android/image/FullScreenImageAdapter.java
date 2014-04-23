package com.cloudpact.mowbly.android.image;

import java.io.IOException;
import java.io.InputStream;
import java.lang.ref.WeakReference;
import java.net.MalformedURLException;
import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.support.v4.view.PagerAdapter;
import android.support.v4.view.ViewPager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.http.Request;
import com.cloudpact.mowbly.android.http.Response;

/**
 * Image Gallery Adapter
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class FullScreenImageAdapter extends PagerAdapter {

	private Activity _activity;
	private ArrayList<String> _imagePaths;
	private ArrayList<String> _imageTitles;
	private LayoutInflater inflater;
	private String title = "";
	private String buttonLabel = "Close";
	private String placeholderImage;
	private String FILE = "file://";
	private String loadingMsg = "";
	private String errorMsg = "";

	// constructor
	public FullScreenImageAdapter(Activity activity,
			ArrayList<String> imagePaths, ArrayList<String> imageTitles) {
		this._activity = activity;
		this._imagePaths = imagePaths;
		this._imageTitles = imageTitles;
	}

	@Override
	public int getCount() {
		return this._imagePaths.size();
	}

	@Override
    public boolean isViewFromObject(View view, Object object) {
        return view == ((RelativeLayout) object);
    }
	
	@Override
    public Object instantiateItem(ViewGroup container, int position) {
		TextView titleDisplay;
		TextView messageDisplay;
        TouchImageView imgDisplay;
        Button btnClose;

        inflater = (LayoutInflater) _activity
                .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View viewLayout = inflater.inflate(R.layout.layout_fullscreen_image, container,
                false);

        imgDisplay = (TouchImageView) viewLayout.findViewById(R.id.imgDisplay);
        btnClose = (Button) viewLayout.findViewById(R.id.btnClose);
        btnClose.setText(buttonLabel);

        String imgTitle = _imageTitles.get(position);
        titleDisplay = (TextView) viewLayout.findViewById(R.id.galleryTitle);
        titleDisplay.setText(imgTitle == null ? title : imgTitle);

        messageDisplay = (TextView) viewLayout.findViewById(R.id.galleryMessage);

        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inPreferredConfig = Bitmap.Config.ARGB_8888;

        Bitmap bitmap;        
        if(placeholderImage != null) { 
        	// Set the placeholder image
        	bitmap = BitmapFactory.decodeFile(_imagePaths.get(position), options);
        	imgDisplay.setImageBitmap(bitmap);
        }

        // Get the image from URL
        new RetriveImageTask(this._activity, imgDisplay, messageDisplay).execute(_imagePaths.get(position));

        // close button click event
        btnClose.setOnClickListener(new View.OnClickListener() {			
			@Override
			public void onClick(View v) {
				_activity.finish();
			}
		}); 

        ((ViewPager) container).addView(viewLayout);
 
        return viewLayout;
	}
	
	@Override
    public void destroyItem(ViewGroup container, int position, Object object) {
        ((ViewPager) container).removeView((RelativeLayout) object);
    }
	
	public void setTitle(String title) {
		this.title = title;
	}
	
	public void setButtonLabel(String label) {
		this.buttonLabel = label;
	}
	
	public void setLoadingMessage(String msg) {
		this.loadingMsg = msg;
	}
	
	public void setErrorMessage(String msg) {
		this.errorMsg = msg;
	}
	
	public void setPlaceholderImage(String placeholder) {
		this.placeholderImage = placeholder;
	}
	
	private class RetriveImageTask extends AsyncTask<String, Void, Bitmap> {

		private WeakReference<TextView> messageViewRef;
		private WeakReference<TouchImageView> imgViewRef;
		
		public RetriveImageTask(Activity context, TouchImageView imgView, TextView messageView) {
			this.imgViewRef = new WeakReference<TouchImageView>(imgView);
			this.messageViewRef = new WeakReference<TextView>(messageView);
		}
		
		@Override
		protected void onProgressUpdate(Void... values) {
			if(this.messageViewRef != null) {
				TextView messageView = this.messageViewRef.get();
				if(messageView != null) {
					messageView.setVisibility(View.VISIBLE);
					messageView.setText(loadingMsg);
				}
			}
		}
		
		@Override
		protected Bitmap doInBackground(String... params) {
			String url = params[0];
			Bitmap bitmap = null;
			this.messageViewRef.get().setText(loadingMsg);
			try {
				BitmapFactory.Options options = new BitmapFactory.Options();
				options.inPurgeable=true;
				options.inInputShareable=true;
				if(url.startsWith("http://") || url.startsWith("https://")){
                     Request request = Request.fromUrl(url);
                     request.setMethod(Request.GET);
                     Response response = request.execute();
                     InputStream is = response.getHttpResponse().getEntity().getContent();
                     bitmap = BitmapFactory.decodeStream(is, null, options);
	             }else{
	                 bitmap = BitmapFactory.decodeFile(url.replace(FILE, ""), options);
	             }
			} catch (MalformedURLException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			} catch (NullPointerException e){
				e.printStackTrace();
			}
			return bitmap;
		}
		
		@Override
		protected void onPostExecute(Bitmap result) {
			if(result != null) {
				if(imgViewRef != null) {
					TouchImageView imgView = imgViewRef.get();
					if(imgView != null) {
						if(this.messageViewRef != null) {
							TextView messageView = this.messageViewRef.get();
							messageView.setVisibility(View.GONE);
						}
						imgView.setImageBitmap(result);
					}
				}
				System.gc();
			} else {
				if(this.messageViewRef != null) {
					TextView messageView = this.messageViewRef.get();
					if(messageView != null) {
						messageView.setVisibility(View.VISIBLE);
						messageView.setText(errorMsg);
					}
				}
			}
		}
	}
}
