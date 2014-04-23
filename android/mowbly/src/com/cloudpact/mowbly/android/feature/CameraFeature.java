package com.cloudpact.mowbly.android.feature;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Iterator;
import java.util.List;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.hardware.Camera;
import android.hardware.Camera.Parameters;
import android.hardware.Camera.Size;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;
import android.provider.MediaStore.Images.ImageColumns;
import android.util.Base64;
import android.util.Pair;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.Intents;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.service.PreferenceService;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the Camera feature.
 * Capture picture or choose from gallery with optional resizing
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>, Aravind Baskaran <aravind@cloudpact.com>
 */
public class CameraFeature extends BaseFeature {

    /** Exposed name of the MessageFeature */
    public static final String NAME = "camera";

    public static final int SOURCE_PHOTO_GALLERY = 0;
    public static final int SOURCE_CAMERA = 1;
    public static final int TYPE_JPG = 0;
    public static final int TYPE_PNG = 1;
    public static final String JPG_TEMP_PIC = "mowbly-temppic.jpg";
    public static final String PNG_TEMP_PIC = "mowbly-temppic.png";
    public static final String CAPTURE_ACTION = "android.media.action.IMAGE_CAPTURE";

    protected static final String TAG = "Camera";
    protected static final Logger logger = Logger.getLogger();

    private boolean readData;
    private JsonObject options;
    private String callbackId = null;
    
    public CameraFeature() {
        super(NAME);
    }

    @TargetApi(9)
    private JsonArray getCameraConfiguration() {
        JsonArray array = new JsonArray();
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.FROYO) {
            Camera c = Camera.open();
            if (c != null) {
                Parameters p = c.getParameters();
                String flMode = p.getFlashMode();
                List<Size> sizes = p.getSupportedPictureSizes();
                c.release();

                JsonArray resolutions = new JsonArray();
                for (Iterator<Size> iterator = sizes.iterator(); iterator.hasNext();) {
                    Size size = (Size) iterator.next();
                    JsonObject s = new JsonObject();
                    s.addProperty("width", size.width);
                    s.addProperty("height", size.height);
                    resolutions.add(s);
                }
                boolean hasFlash = (flMode != null);

                JsonObject prop = new JsonObject();
                prop.addProperty("type", 0);
                prop.add("resolutions", resolutions);
                prop.addProperty("flash", hasFlash);
                array.add(prop);
            }
        } else {
            int noOfCameras = Camera.getNumberOfCameras();
            for (int i = 0; i < noOfCameras; i++) {
                Camera c = Camera.open(i);
                if (c != null) {
                    Parameters p = c.getParameters();
                    String flMode = p.getFlashMode();
                    List<Size> sizes = p.getSupportedPictureSizes();
                    c.release();

                    Camera.CameraInfo cameraInfo = new Camera.CameraInfo(); 
                    Camera.getCameraInfo(i, cameraInfo);
                    int type = (cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_BACK) ? 0 : 1;
                    JsonArray resolutions = new JsonArray();
                    for (Iterator<Size> iterator = sizes.iterator(); iterator.hasNext();) {
                        Size size = (Size) iterator.next();
                        JsonObject s = new JsonObject();
                        s.addProperty("width", size.width);
                        s.addProperty("height", size.height);
                        resolutions.add(s);
                    }
                    boolean hasFlash = (flMode != null);

                    JsonObject prop = new JsonObject();
                    prop.addProperty("type", type);
                    prop.add("resolutions", resolutions);
                    prop.addProperty("flash", hasFlash);
                    array.add(prop);
                }
            }
        }
        return array;
    }

    @Method(async = true, args = {})
    public Response getConfiguration() {
        PreferenceService preferenceService = Mowbly.getPreferenceService();
        JsonArray array = preferenceService.getCameraConfiguration();
        if (array == null) {
            array = getCameraConfiguration();
            preferenceService.setCameraConfiguration(array);
        }

        Response response = new Response();
        response.setResult(array);
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "source", type = int.class),
        @Argument(name = "options", type = JsonObject.class)
    }, callback = true)
    public Response getPicture(int source, JsonObject options, String callbackId) {
    	this.options = getOptions(options);
        this.callbackId = callbackId;
        
        final PageActivity activity = ((FeatureBinder) binder).getActivity();

        Intent intent = new Intent();
        if (source == SOURCE_PHOTO_GALLERY) {
            logger.info(TAG, "Choosing picture from gallery");

            intent.setAction(Intent.ACTION_PICK);
            intent.setData(android.provider.MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            activity.startActivityForResult(intent, Intents.getRequestCode(activity, R.string.action_CAMERA_CHOOSE_PICTURE));
        } else if (source == SOURCE_CAMERA) {
            logger.info(TAG, "Taking picture from camera");

            String suffix = ".jpg";
            if (options.get("type").getAsInt() == TYPE_PNG) {
                suffix = ".png";
            }
            File storageDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES);
            if (!storageDir.exists()) {
                storageDir.mkdirs();
            }
            String imageFileName = "mowbly-temppic" + suffix;
            File image = new File(storageDir, imageFileName);
            intent.setAction(CAPTURE_ACTION);
            intent.putExtra(MediaStore.EXTRA_OUTPUT, Uri.fromFile(image));
            activity.startActivityForResult(intent, Intents.getRequestCode(activity, R.string.action_CAMERA_TAKE_PICTURE));
        }
        return null;
    }
    protected JsonObject getOptions(JsonObject options){
    	if(options != null) {
    		// User asked for edit photo info. Enable store in cache dir.
    		options.addProperty("storeInCacheDir", true);
    	}
        options = (options == null) ? new JsonObject() : options;
        if (options.get("readData") != null) {
            readData = options.get("readData").getAsBoolean();
        } else {
            readData = false;
        }

        if (options.get("width") == null) {
            options.addProperty("width", 320);
        }
        if (options.get("height") == null) {
            options.addProperty("height", 480);
        }
        if (options.get("quality") == null) {
            options.addProperty("quality", 0);
        }
        if (options.get("type") == null) {
            options.addProperty("type", TYPE_JPG);
        }
        return options;
    }

    /**
     * Result handler when application gets focus after Camera/Photo gallery
     * application closes.
     * 
     * Processes the image based on the user options and sends message to JS
     * layer.
     */
    public void onActivityResult(int requestCode, int resultCode, Intent intent) {
        new ProcessPictureTask(requestCode, resultCode, intent).execute();
    }

    protected String getPathForFileOptions(JsonObject options) {
    	return Mowbly.getMowblyFileServiceUtil().getPathForFileOptions(options);
    }

    private class ProcessPictureTask extends AsyncTask<Void, Void, Response> {

        private int requestCode;

        private int resultCode;

        private Intent intent;

        public ProcessPictureTask(int requestCode, int resultCode, Intent intent) {
            this.requestCode = requestCode;
            this.resultCode = resultCode;
            this.intent = intent;
        }

        @Override
        protected Response doInBackground(Void... args) {
            Response response = new Response();
            final PageActivity activity = ((FeatureBinder) binder).getActivity();
            if (requestCode == Intents.getRequestCode(activity, R.string.action_CAMERA_TAKE_PICTURE)) {
                if (resultCode == Activity.RESULT_OK) {
                    response = onTakePictureSuccess(intent);
                } else {
                    logger.info(TAG, "Taking picture activity cancelled");

                    response.setCode(-1);
                    // TODO Find why commented
                    response.setError("Activity cancelled");
                }
            } else if (requestCode == Intents.getRequestCode(activity, R.string.action_CAMERA_CHOOSE_PICTURE)) {
                if (resultCode == Activity.RESULT_OK) {
                    response = onChoosePictureSuccess(intent);
                } else {
                    logger.info(TAG, "Choosing picture activity cancelled");

                    response.setCode(-1);
                    // TODO Find why commented
                    response.setError("Activity cancelled");
                }
            }
            return response;
        }

        @Override
        protected void onPostExecute(Response response) {
            ((FeatureBinder) binder).onAsyncMethodResult(callbackId, response);
            callbackId = null;
        }

        /**
         * Success handler for choosing picture
         * 
         * @param intent The intent
         * @return The result of the action
         */
        private Response onChoosePictureSuccess(Intent intent) {
            Uri receivedUri = intent.getData();

            // Create response. Set the path of the image.
            String imagePath = getFilePathForContentUri(receivedUri);
            Uri uri = Uri.fromFile(new File(imagePath));
            JsonObject returnData = new JsonObject();
            // TODO Check if file:// definitely needs to be added
            returnData.addProperty("path", "file://" + imagePath);

            // Set image data if requested
            Response response = new Response();
            Bitmap bitmap = null;
            final PageActivity pageActivity = ((FeatureBinder) binder).getActivity();
            try {
                int width = options.get("width").getAsInt();
                int height = options.get("height").getAsInt();

                Pair<Integer, Integer> finalDimensions = getFinalImageDimensions(uri, width, height);

                int quality = options.get("quality").getAsInt();
                Bitmap.CompressFormat format = Bitmap.CompressFormat.JPEG;
                if (options.get("type").getAsInt() == TYPE_PNG) {
                    format = Bitmap.CompressFormat.PNG;
                }

                bitmap = BitmapFactory.decodeStream(
                    pageActivity.getContentResolver().openInputStream(uri),
                    null,
                    getBitmapFactoryOptions(uri, finalDimensions.first, finalDimensions.second)
                );
                
                if (readData) {
	                String data = getImageData(bitmap, format, quality);
	                if (data != null) {
	                    returnData.addProperty("data", data);
	                    response.setCode(1);
	                    response.setResult(returnData);
	                } else {
	                    response.setCode(0);
	                    response.setResult(null);
	                    response.setError("Error in compressing picture");
	                }
                } else {
                	// Create the file in filePath if provided, or options if provided
                	// TODO filePath parameter not used to store
                	if(options.get("storeInCacheDir") != null) {
                		String filePath = Mowbly.getFileService().getCacheDir().getPath() + imagePath.substring(imagePath.lastIndexOf("/"));
                		// Write the Bitmap to file
                		try {
                			FileOutputStream fos = new FileOutputStream(filePath);
                			boolean status = bitmap.compress(format, quality, fos);
							fos.close();

							// Set the new path in the data
							if(status) {
								returnData.addProperty("path", "file://" +  filePath);
							}
						} catch (IOException e) {
							logger.error(TAG, "Error writing picture into user location " + filePath + "Reason - " + e.getLocalizedMessage());
						}
                	}
                	
                    response.setCode(1);
                    response.setResult(returnData);
                }
                
            } catch (FileNotFoundException e) {
                logger.error(TAG, "Error occured while reading data from picture " + imagePath + ", Reason - " + " The file by the name is not found");

                response.setCode(0);
                response.setResult(null);
                response.setError("Error in reading picture");
            } catch (OutOfMemoryError e) {
                logger.error(TAG, "Error occured while reading data from picture " + imagePath + ", Reason - " + " The file content is too big to return");

                response.setCode(0);
                response.setResult(null);
                response.setError("Image too big to return");
            } finally {
                if (bitmap != null) {
                    bitmap.recycle();
                    bitmap = null;
                    System.gc();
                }
            }
            return response;
        }

        /**
         * Success handler for taking picture
         * 
         * @param intent The intent
         * @return The result of the action
         */
        @SuppressLint("SimpleDateFormat")
		private Response onTakePictureSuccess(Intent intent) {
            final PageActivity pageActivity = ((FeatureBinder) binder).getActivity();

            String timestamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
            int type = (options.get("type") != null) ? options.get("type").getAsInt() : TYPE_JPG;
            String suffix = (type == TYPE_PNG) ? ".png" : ".jpg";
            File storageDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES);

            String tmpFileName = "mowbly-temppic" + suffix;
            File tmpImage = new File(storageDir, tmpFileName);
            Uri pictureUri = Uri.fromFile(tmpImage);

            Response response = new Response();
            Uri contentUri;
            if (options.get("filePath") != null && options.get("filePath").isJsonObject()) {
                JsonObject fileOptions = options.get("filePath").getAsJsonObject();
                String filePath = getPathForFileOptions(fileOptions);
                if (filePath.startsWith("file:///")) {
                    filePath = filePath.substring(7);
                }
                File imageFile = new File(filePath);
                if(!imageFile.exists()){
	                try {
	                	imageFile.getParentFile().mkdirs();
	                	imageFile.createNewFile();
					} catch (IOException e) {
						logger.error(TAG, "Error occured while storing picture, Reason - " + e.getMessage());
						response.setCode(0);
		                response.setError("Error in saving picture");
		                return response;
					}
                }
                contentUri = Uri.fromFile(imageFile);
            } else {
                String imageFileName = "mowbly-" + timestamp + suffix;
                File finalImage = new File(storageDir, imageFileName);
                contentUri = Uri.fromFile(finalImage);
            }
            String imagePath = contentUri.toString();

            Bitmap bitmap = null;
            try {
                int width = options.get("width").getAsInt();
                int height = options.get("height").getAsInt();

                int quality = options.get("quality").getAsInt();
                Bitmap.CompressFormat format = Bitmap.CompressFormat.JPEG;
                if (options.get("type").getAsInt() == TYPE_PNG) {
                    format = Bitmap.CompressFormat.PNG;
                }

                // Get the taken image
                // Process the image [scale] as per user options and get bitmap
                InputStream istream = pageActivity.getContentResolver().openInputStream(pictureUri);
                OutputStream os = pageActivity.getContentResolver().openOutputStream(contentUri);
                if (width == -1 && height == -1) {
                    byte[] buffer = new byte[1024];
                    int numRead = 0;
                    while ((numRead = istream.read(buffer, 0, buffer.length)) != -1) {
                        os.write(buffer, 0, numRead);
                    }
                    istream.close();
                } else {
                    Pair<Integer, Integer> finalDimensions = getFinalImageDimensions(pictureUri, width, height);

                    bitmap = BitmapFactory.decodeStream(istream, null, getBitmapFactoryOptions(pictureUri, finalDimensions.first, finalDimensions.second));
                    //bitmap = getCorrectlyOrientedImage(pageActivity, pictureUri, finalDimensions.first, finalDimensions.second);
                    bitmap.compress(format, quality, os);
                }
                os.flush();
                os.close();

                // Prepare and return the response
                String imageData = "";
                if (readData) {
                    // Read the file data and encode as base64.
                    InputStream is = pageActivity.getContentResolver().openInputStream(contentUri);
                    ByteArrayOutputStream baos = new ByteArrayOutputStream();
                    byte[] buffer = new byte[1024];
                    int numRead = 0;
                    while ((numRead = is.read(buffer, 0, buffer.length)) != -1) {
                        baos.write(buffer, 0, numRead);
                    }
                    imageData = Base64.encodeToString(baos.toByteArray(), 0);
                    baos.close();
                    is.close();
                }

                JsonObject returnData = new JsonObject();
                returnData.addProperty("path", imagePath);
                returnData.addProperty("data", imageData);
                response.setCode(1);
                response.setResult(returnData);
            } catch (Exception e) {
                logger.error(TAG, "Error occured while taking picture, Reason - " + e.getMessage());

                response.setCode(0);
                response.setError("Error in saving picture");
            } finally {
                if (bitmap != null) {
                    bitmap.recycle();
                    bitmap = null;
                    System.gc();
                }
                tmpImage.delete();
            }
            return response;
        }

        private Pair<Integer, Integer> getImageDimensions(Uri uri) {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.inJustDecodeBounds = true;
            BitmapFactory.decodeFile(uri.getPath(), options);
            int imageHeight = options.outHeight;
            int imageWidth = options.outWidth;
            return new Pair<Integer, Integer>(imageWidth, imageHeight);
        }

        private Pair<Integer, Integer> getFinalImageDimensions(Uri uri, int width, int height) {
            if (width != -1 && height != -1) {
                return new Pair<Integer, Integer>(width, height);
            }

            Pair<Integer, Integer> originalDimensions = getImageDimensions(uri);
            if(originalDimensions.first == 0 || originalDimensions.second == 0){
            	logger.warn(TAG, "Unable to get dimensions for URI - " + uri);
                return new Pair<Integer, Integer>(width, height);
            }

            if (width == -1 && height == -1) {
                // Both auto
                return originalDimensions;
            } else if (width == -1) {
                // Width auto
                int finalWidth = (originalDimensions.first * height) / originalDimensions.second;
                return new Pair<Integer, Integer>(finalWidth, height);
            } else {
                // Height auto
                int finalHeight = (width * originalDimensions.second) / originalDimensions.first;
                return new Pair<Integer, Integer>(width, finalHeight);
            }
        }

        /**
         * Returns the file path for the content uri.
         * 
         * @param uri The uri of the content
         * @return The file path corresponding to the content uri
         */
        private String getFilePathForContentUri(Uri uri) {
            final PageActivity pageActivity = ((FeatureBinder) binder).getActivity();

            Cursor c = pageActivity.getContentResolver().query(uri, null, null, null, null);
            String path = uri.toString();
            if (c.moveToFirst()) {
                path = c.getString(c.getColumnIndex(ImageColumns.DATA));
            }
            return path;
        }

        /**
         * Get the Uri for the image content in media store.
         * 
         * @param imageType The type of the image which decides the mime-type to set
         * @return The Uri of the media store
         */
        /*
        private Uri getContentUri(int imageType) {
            final PageActivity pageActivity = ((FeatureBinder) binder).getActivity();

            ContentValues values = new ContentValues();
            String mimeType = (imageType == TYPE_JPG) ? "image/jpeg" : "image/png";
            values.put(android.provider.MediaStore.Images.Media.MIME_TYPE, mimeType);
            Uri uri = null;
            try {
                uri = pageActivity.getContentResolver().insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
            } catch (UnsupportedOperationException e) {
                try {
                    uri = pageActivity.getContentResolver().insert(MediaStore.Images.Media.INTERNAL_CONTENT_URI, values);
                } catch (UnsupportedOperationException ex) {
                }
            }

            return uri;
        }
        */

        /**
         * Returns the data of the image as a base64 encoded string.
         * 
         * @param bitmap The bitmap content of the image
         * @param format The compression format which decides the compression of bitmap
         * @param quality The required compression quality of the image
         * @return The base64 encoded string data of the image
         */
        private String getImageData(Bitmap bitmap, Bitmap.CompressFormat format, int quality) {
            ByteArrayOutputStream data = new ByteArrayOutputStream();
            String dataStr = null;
            if (bitmap.compress(format, quality, data)) {
                byte[] code = data.toByteArray();
                dataStr = Base64.encodeToString(code, 0);
                code = null;
            }
            data = null;
            return dataStr;
        }

        /**
         * Returns the bitmap options with the sample size calculated based on the
         * factor of actual image dimensions and user required image dimensions.
         * 
         * @param uri The uri of the image
         * @param width The width of the image
         * @param height The height of the image
         * @return The bitmap factory options with the samplesize factor
         */
        private BitmapFactory.Options getBitmapFactoryOptions(Uri uri, int width, int height) {
            final PageActivity pageActivity = ((FeatureBinder) binder).getActivity();

            BitmapFactory.Options o = new BitmapFactory.Options();
            o.inJustDecodeBounds = true;
            o.inDither = false;
            o.inPurgeable = true;
            o.inInputShareable = true;
            o.inTempStorage = new byte[32 * 800];

            int scale = 1;
            try {
                BitmapFactory.decodeStream(pageActivity.getContentResolver().openInputStream(uri), null, o);

                // Determine the scale
                int iWidth = o.outWidth;
                int iHeight = o.outHeight;

                while (width < iWidth && height < iHeight) {
                    iWidth /= 2;
                    iHeight /= 2;
                    scale *= 2;
                }
            } catch (Exception e) {
            }

            // Set the revised options
            o.inJustDecodeBounds = false;
            o.inSampleSize = scale;

            return o;
        }

        /*
        private int getOrientation(Context context, Uri photoUri) throws IOException {
            ExifInterface exif = new ExifInterface(photoUri.getPath());
            String orientation =  exif.getAttribute(ExifInterface.TAG_ORIENTATION);
            
            return Integer.parseInt(orientation);
        }

        private Bitmap getCorrectlyOrientedImage(Context context, Uri photoUri, int width, int height) throws IOException {
            InputStream is = context.getContentResolver().openInputStream(photoUri);
            BitmapFactory.Options dbo = getBitmapFactoryOptions(photoUri, width, height);
            //dbo.inJustDecodeBounds = true;
            Bitmap srcBitmap = BitmapFactory.decodeStream(is, null, dbo);
            is.close();

            //int rotatedWidth, rotatedHeight;
            int orientation = getOrientation(context, photoUri);
            
            if(orientation == 3)
                orientation = 180;
            else if(orientation == 6)
                orientation = 90;
            else if(orientation == 8)
                orientation = 270;

            // if the orientation is not 0 (or -1, which means we don't know), we have to do a rotation.
            if (orientation > 0) {
                Matrix matrix = new Matrix();
                matrix.postRotate(orientation);

                srcBitmap = Bitmap.createBitmap(srcBitmap, 0, 0, srcBitmap.getWidth(),
                        srcBitmap.getHeight(), matrix, true);
            }

            return srcBitmap;
        }
        */
    }
}
