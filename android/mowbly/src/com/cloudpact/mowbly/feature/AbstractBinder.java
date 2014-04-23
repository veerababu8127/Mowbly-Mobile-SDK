package com.cloudpact.mowbly.feature;

import java.util.Vector;

/**
 * Abstract implementation of the {@link Binder}
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
abstract public class AbstractBinder implements Binder {

    protected static final int LIFE_CYCLE_ACTION_CREATE = 1;

    protected static final int LIFE_CYCLE_ACTION_START = 2;

    protected static final int LIFE_CYCLE_ACTION_RESUME = 3;

    protected static final int LIFE_CYCLE_ACTION_PAUSE = 4;

    protected static final int LIFE_CYCLE_ACTION_STOP = 5;

    protected static final int LIFE_CYCLE_ACTION_DESTROY = 6;

    /** The name associated with the binder */
    protected String name;

    /** The list of features binded */
    protected Vector<Feature> features;

    protected AbstractBinder(String name) {
        setName(name);
    }

    public String getName() {
        return name;
    }

    public void setName(String binderName) {
        name = binderName;
    }

    public void bind(Feature feature) {
        feature.setBinder(this);
        if (features == null) {
            features = new Vector<Feature>();
        }
        features.addElement(feature);
    }

    public Feature get(String featureName) {
        Feature feature = null;
        if (features != null) {
            int size = features.size();
            for (int i = 0; i < size; i++) {
                feature = (Feature) features.elementAt(i);
                if (feature.getName().equals(featureName)) {
                    return feature;
                }
            }
        }
        return null;
    }

    public void onCreate() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_CREATE);
    }

    public void onStart() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_START);
    }

    public void onResume() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_RESUME);
    }

    public void onPause() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_PAUSE);
    }

    public void onStop() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_STOP);
    }

    public void onDestroy() {
        callLifeCycleAction(LIFE_CYCLE_ACTION_DESTROY);
    }

    /**
     * Bind a list of features to the binder
     * 
     * @param features The list of features to bind
     */
    public void bind(Vector<Feature> features) {
        if (features != null) {
            Feature feature;
            int size = features.size();
            for (int i = 0; i < size; i++) {
                feature = (Feature) features.elementAt(i);
                bind(feature);
            }
        }
    }

    /**
     * Perform a life cycle action on all the feature bound
     * 
     * @param action The action to be performed
     */
    private void callLifeCycleAction(int action) {
        if (features != null) {
            int size = features.size();
            for (int i = 0; i < size; i++) {
                Feature feature = (Feature) features.elementAt(i);
                switch (action) {
                case LIFE_CYCLE_ACTION_CREATE:
                    feature.onCreate();
                    break;
                case LIFE_CYCLE_ACTION_START:
                    feature.onStart();
                    break;
                case LIFE_CYCLE_ACTION_RESUME:
                    feature.onResume();
                    break;
                case LIFE_CYCLE_ACTION_PAUSE:
                    feature.onPause();
                    break;
                case LIFE_CYCLE_ACTION_STOP:
                    feature.onStop();
                    break;
                case LIFE_CYCLE_ACTION_DESTROY:
                    feature.onDestroy();
                    break;
                }
            }
        }
    }
}
