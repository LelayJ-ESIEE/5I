package com.esiee.tp.web.rest;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;
import java.util.Optional;

import javax.inject.Inject;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.esiee.tp.domain.Function;
import com.esiee.tp.repository.FunctionRepository;
import com.esiee.tp.web.rest.util.HeaderUtil;

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
	
	@RequestMapping(value = "/functions/{id}", method = RequestMethod.GET, produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Function> getFunction(@PathVariable Long id){
		log.debug("REST request to get Function: {}", id);
		Function function = functionRepository.findOne(id);
		return Optional.ofNullable(function)
				.map(result -> new ResponseEntity<>(
						result,
						HttpStatus.OK))
				.orElse(new ResponseEntity<>(HttpStatus.NOT_FOUND));
	}
	
	@RequestMapping(value = "/functions", method = RequestMethod.POST, produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Function> createFunction(@RequestBody Function function) throws URISyntaxException {
		log.debug("REST request to save Function: {}", function);
		if (function.getId() != null)
				return ResponseEntity.badRequest().headers(HeaderUtil.createFailureAlert("function", "idexists", "A new function cannot already have an ID")).body(null);
		Function result = functionRepository.save(function);
		return ResponseEntity.created(new URI("/api/functions/" + result.getId()))
				.headers(HeaderUtil.createEntityCreationAlert("function", result.getId().toString()))
				.body(result);
	}
	
	@RequestMapping(value = "functions", method = RequestMethod.PUT, produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Function> updateFunction(@RequestBody Function function) throws URISyntaxException {
		log.debug("REST request to update Function: {}", function);
		if (function.getId() == null)
			return createFunction(function);
		Function result = functionRepository.save(function);
		return ResponseEntity.ok()
				.headers(HeaderUtil.createEntityUpdateAlert("function", function.getId().toString()))
				.body(result);
	}
	
	@RequestMapping(value = "/functions/{id}", method = RequestMethod.DELETE, produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Void> deleteFunction(@PathVariable Long id) {
		log.debug("REST request to delete Function: {}", id);
		functionRepository.delete(id);
		return ResponseEntity.ok().headers(HeaderUtil.createEntityDeletionAlert("function", id.toString())).build();
	}
}
