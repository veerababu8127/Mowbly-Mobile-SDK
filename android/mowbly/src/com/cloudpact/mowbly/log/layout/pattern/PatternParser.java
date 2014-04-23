package com.cloudpact.mowbly.log.layout.pattern;

import java.util.Vector;

import com.cloudpact.mowbly.log.layout.SimpleLayout;
import com.cloudpact.mowbly.log.layout.pattern.formatter.DatePatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.FormatInformation;
import com.cloudpact.mowbly.log.layout.pattern.formatter.LevelPatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.LiteralPatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.MessagePatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.PatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.TagPatternFormatter;
import com.cloudpact.mowbly.log.layout.pattern.formatter.TypePatternFormatter;

/**
 * Parses the given pattern to give a linked list of formatters
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PatternParser {

    final static private char ESCAPE_CHAR = '%';

    /*
    static private enum STATE {
        LITERAL, CONVERTER, DOT, MIN, MAX
    }
    */
    private static final int STATE_LITERAL = 1;
    private static final int STATE_CONVERTER = 2;
    private static final int STATE_DOT = 3;
    private static final int STATE_MIN = 4;
    private static final int STATE_MAX = 5;

    private String pattern;

    private int patternLength;

    protected FormatInformation fi = new FormatInformation();

    private Vector<PatternFormatter> formatters;

    private int state = STATE_LITERAL;

    private int i = 0;

    protected StringBuffer currentLiteral = new StringBuffer(32);

    public PatternParser(String pattern) {
        setPattern(pattern);
    }

    public String getPattern() {
        return pattern;
    }

    public void setPattern(String pattern) {
        this.pattern = pattern;
        this.patternLength = this.pattern.length();
    }

    public Vector<PatternFormatter> parse() {
        formatters = new Vector<PatternFormatter>();

        char c;
        while (i < patternLength) {
            c = pattern.charAt(i++);
            switch (state) {
                case STATE_LITERAL:
                    // In literal state, the last char is always a literal.
                    if (i == patternLength) {
                        currentLiteral.append(c);
                        continue;
                    }
    
                    if (c == ESCAPE_CHAR) {
                        // peek at the next char.
                        switch (pattern.charAt(i)) {
                            case ESCAPE_CHAR:
                                currentLiteral.append(c);
                                i++;
                                break;
                            case 'n':
                                currentLiteral.append(SimpleLayout.LINE_SEPARATOR);
                                i++;
                                break;
                            default:
                                if (currentLiteral.length() != 0) {
                                    formatters.addElement(new LiteralPatternFormatter(currentLiteral.toString()));
                                }
                                currentLiteral.setLength(0);
                                currentLiteral.append(c);
                                state = STATE_CONVERTER;
                                fi.reset();
                                break;
                        }
                    } else {
                        currentLiteral.append(c);
                    }
                    break;
                case STATE_CONVERTER:
                    currentLiteral.append(c);
                    switch (c) {
                        case '-':
                            fi.setLeftAlign(true);
                            break;
                        case '.':
                            state = STATE_DOT;
                            break;
                        default:
                            if (c >= '0' && c <= '9') {
                                fi.setMin(c - '0');
                                state = STATE_MIN;
                            } else {
                                addFormatter(c);
                            }
                            break;
                    }
                    break;
                case STATE_DOT:
                    currentLiteral.append(c);
                    if (c >= '0' && c <= '9') {
                        fi.setMax(c - '0');
                        state = STATE_MAX;
                    } else {
                        // Error occured. Was expecting digit, instead got char
                        state = STATE_LITERAL;
                    }
                    break;
                case STATE_MIN:
                    currentLiteral.append(c);
                    if (c >= '0' && c <= '9') {
                        fi.setMin(fi.getMin() * 10 + (c - '0'));
                    } else if (c == '.') {
                        state = STATE_DOT;
                    } else {
                        addFormatter(c);
                    }
                    break;
                case STATE_MAX:
                    currentLiteral.append(c);
                    if (c >= '0' && c <= '9') {
                        fi.setMax(fi.getMax() * 10 + (c - '0'));
                    } else {
                        addFormatter(c);
                    }
                    break;
            }
        }

        if (currentLiteral.length() != 0) {
            formatters.addElement(new LiteralPatternFormatter(currentLiteral.toString()));
        }

        return formatters;
    }

    protected void addFormatter(char c) {
        PatternFormatter pf = getPatternFormatterForChar(c);
        currentLiteral.setLength(0);
        formatters.addElement(pf);
        state = STATE_LITERAL;
        fi.reset();
    }

    protected PatternFormatter getPatternFormatterForChar(char c){
    	PatternFormatter pf = null;
    	switch (c) {
	        case 'd':
	            String dateFormatStr = extractOption();
	            pf = new DatePatternFormatter(fi, dateFormatStr);
	            break;
	        case 'w':
	            pf = new TypePatternFormatter(fi);
	            break;
	        case 'l':
	            pf = new LevelPatternFormatter(fi);
	            break;
	        case 't':
	            pf = new TagPatternFormatter(fi);
	            break;
	        case 'm':
	            pf = new MessagePatternFormatter(fi);
	            break;
	        default:
	            pf = new LiteralPatternFormatter(currentLiteral.toString());
	            break;
	    }
    	return pf;
    }
    
    private String extractOption() {
        if ((i < patternLength) && (pattern.charAt(i) == '{')) {
            int end = pattern.indexOf('}', i);
            if (end > i) {
                String r = pattern.substring(i + 1, end);
                i = end+1;
                return r;
            }
        }
        return null;
    }
}
