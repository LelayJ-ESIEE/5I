/**
 * 
 */
package com.esiee.tp2.model;

import com.esiee.tp2.domain.Civility;
import com.esiee.tp2.domain.Function;
import com.esiee.tp2.domain.Person;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.HashMap;

/**
 * @author LelayJ-ESIEE
 *
 */
public class Datamodel {
	private Map<Long, Civility> civilities = new HashMap<Long, Civility>();
	private Map<Long, Function> functions = new HashMap<Long, Function>();
	private Map<Long, Person> persons = new HashMap<Long, Person>();
	
	/** Constructeur privé */
	private Datamodel() {
		initData();
	}

	/** Holder */
    private static class DatamodelHolder
    {       
    	/** Instance unique non préinitialisée */
        private final static Datamodel instance = new Datamodel();
    }
 
    /** Point d'accès pour l'instance unique du singleton */
    public static Datamodel getInstance()
    {
        return DatamodelHolder.instance;
    }
	
	/** Initialisation du Datamodel */
    private void initData() {
		initCivilities();
		initFunctions();
		initPersons();
	}

	private void initCivilities() {
		Civility civilityOne = new Civility();
		civilityOne.setId(0);
		civilityOne.setCode("M");
		civilityOne.setLabel("Monsieur");
		civilities.put(civilityOne.getId(), civilityOne);
		
		Civility civilityTwo = new Civility();
		civilityOne.setId(1);
		civilityOne.setCode("Mme");
		civilityOne.setLabel("Madame");
		civilities.put(civilityTwo.getId(), civilityTwo);
		
		Civility civilityThree = new Civility();
		civilityThree.setId(2);
		civilityThree.setCode("Mx");
		civilityThree.setLabel("Mixe");
	}

	private void initFunctions() {
		Function functionOne = new Function();
		functionOne.setId(0);
		functionOne.setCode("Dr.");
		functionOne.setLabel("Docteur");
		functions.put(functionOne.getId(), functionOne);
		
	}

	private void initPersons() {
		Person personOne = new Person();
		personOne.setId(0);
		personOne.setFirstname("Carla");
		personOne.setLastname("Aldebert");
		personOne.setCivility(civilities.get(new Long(1)));
		personOne.setFunction(functions.get(new Long(0)));
		personOne.setLogin("aldeberc");
		personOne.setPassword("123456");
		personOne.setMail("carla.aldebert@mail.null");
		personOne.setMobilePhone("+33 6 12 34 45 67");
		persons.put(personOne.getId(), personOne);
		
		Person personTwo = new Person();
		personTwo.setId(1);
		personTwo.setFirstname("Dorian");
		personTwo.setLastname("Brachet");
		personTwo.setCivility(civilities.get(new Long(2)));
		personTwo.setFunction(null);
		personTwo.setLogin("brachetd");
		personTwo.setPassword("17102020");
		personTwo.setMail("dorian.brachet@gmail.com");
		personTwo.setMobilePhone("+33 6 15 93 67 68");
		persons.put(personTwo.getId(), personTwo);
	}

    /**
     * Retourne la totalité des personnes présentes dans le model de données.
     * @return
     */
    List<Person> getPersons() {
    	Set<Long> keys = persons.keySet();
    	List<Person> personsList = new ArrayList<Person>();
    	for (Long key: keys)
    		personsList.add(persons.get(key));
    	return personsList;
    }
    
    /**
     * Retourne la personne correspondant à l’id
     * @param id
     * @return
     */
    Person getPerson(Long id) {
    	return this.persons.get(id);
    }
    
    /**
     * Retourne la totalité des civilités présentes dans le model de données.
     * @return
     */
    List<Civility> getCivilities() {
    	Set<Long> keys = civilities.keySet();
    	List<Civility> civilitiesList = new ArrayList<Civility>();
    	for (Long key: keys)
    		civilitiesList.add(civilities.get(key));
    	return civilitiesList;
    }
    
    /**
     * Retourne la civilité correspondant à l’id
     * @param id
     * @return
     */
    Civility getCivility(Long id) {
    	return this.civilities.get(id);
    }
    
    /**
     * Retourne la totalité des fonctions présentes dans le model de données.
     * @return
     */
    List<Function> getFunctions() {
    	Set<Long> keys = functions.keySet();
    	List<Function> functionsList = new ArrayList<Function>();
    	for (Long key: keys)
    		functionsList.add(functions.get(key));
    	return functionsList;
    }
    
    /**
     * Retourne la fonction correspondant à l’id
     * @param id
     * @return
     */
    Function getFunction(Long id) {
    	return this.functions.get(id);
    }
}
