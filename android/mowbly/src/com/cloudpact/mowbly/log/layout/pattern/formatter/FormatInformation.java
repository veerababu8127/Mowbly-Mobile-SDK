package com.cloudpact.mowbly.log.layout.pattern.formatter;

/**
 * FormatInformation
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FormatInformation {

    private int min;

    private int max;

    private boolean leftAlign;

    public FormatInformation() {
        reset();
    }

    public void reset() {
        min = -1;
        max = 0x7FFFFFFF;
        leftAlign = false;
    }

    public int getMin() {
        return min;
    }

    public void setMin(int min) {
        this.min = min;
    }

    public int getMax() {
        return max;
    }

    public void setMax(int max) {
        this.max = max;
    }

    public boolean isLeftAlign() {
        return leftAlign;
    }

    public void setLeftAlign(boolean leftAlign) {
        this.leftAlign = leftAlign;
    }
}
