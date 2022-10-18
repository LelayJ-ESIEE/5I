package com.esiee.tp.web.rest;

import java.io.Serializable;

import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class AboutResource {
	
	@RequestMapping(value = "/about", method = RequestMethod.GET, produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<About> getAbout() {
	
		ResponseEntity<About> responseEntity = null;
		
		About about = new About();
		about.setVersion("1.0.0");
		
		responseEntity = new ResponseEntity<AboutResource.About>(about, HttpStatus.OK);
		return responseEntity;
	}
	
	private class About implements Serializable {
		
		private static final long serialVersionUID = 1L;
		private String version;
		
		public String getVersion() {
			return version;
		}
		
		public void setVersion(String version) {
			this.version = version;
		}

	}

}
