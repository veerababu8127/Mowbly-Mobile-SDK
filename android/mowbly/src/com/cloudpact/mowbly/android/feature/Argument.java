package com.cloudpact.mowbly.android.feature;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * FeatureMethodArgument annotation
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.METHOD)
public @interface Argument {

    String name();

    Class<?> type();
}
