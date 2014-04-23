package com.cloudpact.mowbly.log;

/**
 * Log Priority
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Priority implements Comparable<Priority> {

    final static protected int DEBUG_PRIORITY = 10000;

    final static protected int INFO_PRIORITY = 20000;

    final static protected int WARN_PRIORITY = 30000;

    final static protected int ERROR_PRIORITY = 40000;

    final static protected int FATAL_PRIORITY = 50000;

    private int priority;

    private String priorityStr;

    protected Priority(int priority, String priorityStr) {
        this.priority = priority;
        this.priorityStr = priorityStr;
    }

    public int getPriority() {
        return priority;
    }

    public boolean isGreaterOrEqualTo(Priority priority) {
        return (compareTo(priority) >= 0);
    }

    public String toString() {
        return priorityStr;
    }

    public boolean equals(Object obj) {
        if (obj instanceof Priority) {
            Priority p = (Priority) obj;
            return (compareTo(p) == 0);
        }
        return false;
    }

    public int compareTo(Priority obj) {
        int bPriority = obj.getPriority();
        if (priority < bPriority) {
            return -1;
        } else if (priority > bPriority) {
            return 1;
        } else {
            return 0;
        }
    }
}
