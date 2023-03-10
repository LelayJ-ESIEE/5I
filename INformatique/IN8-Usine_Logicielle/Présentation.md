## Présentation de ma (maigre) performance en Usine Logicielle

#### De l'importance de la préparation avant un examen

---

## Les problèmes

* Commencer par une connexion lente
* Ajouter une erreur fatale dûe au stress : rm -rf / malencontreux
* Terminez par une panne matérielle
![[Panne.mov]]
---

## Chargement des images

Après le téléchargement, chargement des images :
```
cat sme-eval.tar.gz | docker load
cat sme-git.tar.gz | docker load
cat sme-jenkins.tar.gz | docker load
cat sme-sonar.tar.gz | docker load
```

Démarrage du container eval : 
```
docker run  -d sme/eval
```

---

## GitLab

### Démarrage du container eval-git
```
docker run -d --hostname git -P -p 55010:80 --name eval-git --volume $(pwd)/git/gitlab/config:/etc/gitlab --volume $(pwd)/git/gitlab/logs:/var/log/gitlab --volume $(pwd)/git/gitlab/data:/var/opt/gitlab sme/git
```

### Configuration
Ajout des adresses aux fichiers hosts des containers
Connection (mot de passe obtenu grâce à `cat /etc/gitlab/initial_root_password`)
Modification du mot de passe de l'utilisateur root

---

## GitLab

### Versionnage
Création du dépôt sous GitLab
Versioning du code
```
git init
git remote add origin http://localhost:55010/gitlab-instance-{id}/eval-git.git
git add *
git commit -m "initial commit"
git push --set-upstream origin master
```

---

## Jenkins

### Démarrage du container eval-jenkins
```
docker run -d -p 8080:8080 -p 50000:50000 -v jenkins_home:/var/jenkins_home --name eval-jenkins sme/jenkins
```

---

## Jenkins

Connexion (mot de passe obtenu grâce à `cat /var/jenkins_home/secrets/initialAdminPassword`)