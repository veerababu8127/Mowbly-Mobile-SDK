package com.cloudpact.mowbly.android.util;

import java.io.UnsupportedEncodingException;

import android.util.Base64;

/**
 * Encoding utilities
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class EncodingUtils {

    private EncodingUtils() {
    }

    /**
     * Decode base64 encoded string
     *
     * @param content The base64 encoded string which is to be decoded 
     * @return The decoded byte array
     */
    public static final byte[] fromBase64(String content) {
        return Base64.decode(content, Base64.DEFAULT);
    }

    /**
     * Base64 encode given byte array
     *
     * @param content
     * @return byte array
     */
    public static final String toBase64(byte[] content) {
        return Base64.encodeToString(content, Base64.DEFAULT);
    }

    /**
     * Base64 encode the given string
     *
     * @param content The string to encode in Base64
     * @return The Base64 encoded string
     */
    public static final String toBase64(String content) {
        byte[] bytes;
        try {
            bytes = content.getBytes("UTF-8");
        } catch (UnsupportedEncodingException e) {
            bytes = content.getBytes();
        }
        return toBase64(bytes);
    }
}
