package com.esiee.tp.web.rest;

import java.util.List;

import javax.inject.Inject;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.esiee.tp.domain.Function;
import com.esiee.tp.repository.FunctionRepository;

@RestController
@RequestMapping("/api")
public class FunctionResource {
	
	private final Logger log = LoggerFactory.getLogger(FunctionResource.class);
	
	@Inject
	private FunctionRepository functionRepository;
	
	@RequestMapping(value = "/funcitons", method = RequestMethod.GET, produces = MediaType.APPLICATION_JSON_VALUE)
	public List<Function> getAllFunctions() {
		log.debug("REST request to get all Functions");
		List<Function> functions = functionRepository.findAll();
		return functions;
	}
	
}
