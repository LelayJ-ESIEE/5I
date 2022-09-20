/**
 * 
 */
package com.esiee.tp2.domain;

/**
 * @author LelayJ-ESIEE
 *
 */
public class Person {
	private long id;
	private String lastname;
	private String firstname;
	private String mail;
	private String mobilePhone;
	private String login;
	private String password;
	private Civility civility;
	private Function function;
	
	public Person() {
		this.id = 0;
		this.lastname = "";
		this.firstname = "";
		this.mail = "";
		this.mobilePhone = "";
		this.login = "";
		this.password = "";
		this.civility = null;
		this.function = null;
	}
	
	public long getId() {
		return id;
	}
	public String getLastname() {
		return lastname;
	}	
	public String getFirstname() {
		return firstname;
	}
	public String getMail() {
		return mail;
	}
	public String getMobilePhone() {
		return mobilePhone;
	}
	public String getLogin() {
		return login;
	}
	public String getPassword() {
		return password;
	}
	public Civility getCivility() {
		return civility;
	}
	public Function getFunction() {
		return function;
	}

	public void setId(long id) {
		this.id = id;
	}
	public void setLastname(String lastname) {
		this.lastname = lastname;
	}
	public void setFirstname(String firstname) {
		this.firstname = firstname;
	}
	public void setMail(String mail) {
		this.mail = mail;
	}
	public void setMobilePhone(String mobilePhone) {
		this.mobilePhone = mobilePhone;
	}
	public void setLogin(String login) {
		this.login = login;
	}
	public void setPassword(String password) {
		this.password = password;
	}
	public void setCivility(Civility civility) {
		this.civility = civility;
	}
	public void setFunction(Function function) {
		this.function = function;
	}
	
}
