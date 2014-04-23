package com.cloudpact.mowbly.android.feature;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * FeatureMethod annotation
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.METHOD)
public @interface Method {

    boolean async() default false;

    Argument[] args() default {};
    
    boolean callback() default false;
}
