package com.cloudpact.mowbly.android.page;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonParser;

/**
 * Object representing a Page
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Page implements Parcelable {

    protected String name;

    protected String url;

    protected String parent;

    protected String configuration;

    protected String data;

    protected String result;
    
    public Page(){}
    
    public Page(String name, String url) {
        setName(name);
        setUrl(url);
    }

    public String getName() {
        return name;
    }

    public Page setName(String name) {
        this.name = name;
        return this;
    }

    public String getUrl() {
        return url;
    }

    public Page setUrl(String url) {
        this.url = url;
        return this;
    }

    public String getParent() {
        return parent;
    }

    private Page setParent(String parent) {
        this.parent = parent;
        return this;
    }

    public Page setParent(Page parentPage) {
        if (parentPage != null) {
            setParent(parentPage.getName());
        }
        return this;
    }
    
    public String getConfiguration() {
        return configuration;
    }

    public Page setConfiguration(String configuration) {
        this.configuration = configuration;
        return this;
    }

    public String getData() {
        return data;
    }

    public Page setData(String data) {
        this.data = data;
        return this;
    }

    public String getResult() {
        return this.result;
    }

    public Page setResult(String result) {
        this.result = result;
        return this;
    }

    protected JsonObject getConfig() {
        JsonObject config = null;
        if (configuration != null) {
            try {
                config = new JsonParser().parse(configuration)
                        .getAsJsonObject();
            } catch (JsonParseException e) {
            }
        }
        return config;
    }

    public boolean isRetainedInViewStack() {
        boolean isRetainedInViewStack = true;
        JsonObject config = getConfig();
        if (config != null && config.get("retainPageInViewStack") != null) {
            isRetainedInViewStack = config.get("retainPageInViewStack").getAsBoolean();
        }
        return isRetainedInViewStack;
    }
    
    @Override
    public boolean equals(Object other) {
        if (other == this) {
            return true;
        }

        if (!(other instanceof Page)) {
            return false;
        }
        return getName().equals(((Page) other).getName());
    }

    @Override
    public String toString() {
        return getClass().getSimpleName() + "[" + getName() + "]";
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeString(name);
        dest.writeString(url);
        dest.writeString(parent);
        dest.writeString(configuration);
        dest.writeString(data);
        dest.writeString(result);
    }

    public static final Parcelable.Creator<Page> CREATOR = new Parcelable.Creator<Page>() {
        public Page createFromParcel(Parcel in) {
            return new Page(in);
        }

        public Page[] newArray(int size) {
            return new Page[size];
        }
    };

    protected Page(Parcel in) {
        setName(in.readString());
        setUrl(in.readString());
        setParent(in.readString());
        setConfiguration(in.readString());
        setData(in.readString());
        setResult(in.readString());
    }
}
