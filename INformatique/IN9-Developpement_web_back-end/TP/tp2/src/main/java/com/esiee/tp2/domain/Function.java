/**
 * 
 */
package com.esiee.tp2.domain;

/**
 * @author LelayJ-ESIEE
 *
 */
public class Function {
	private long id;
	String code;
	String label;
	
	public Function() {
		this.id = 0;
		this.code = "";
		this.label = "";
	}

	public long getId() {
		return id;
	}
	public String getCode() {
		return code;
	}
	public String getLabel() {
		return label;
	}

	public void setId(long id) {
		this.id = id;
	}
	public void setCode(String code) {
		this.code = code;
	}
	public void setLabel(String label) {
		this.label = label;
	}
	
}
