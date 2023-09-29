pipeline {
    environment {
      branchname =  env.BRANCH_NAME.toLowerCase()
      kubeconfig = getKubeconf(env.branchname)
      registryCredential = 'jenkins_registry'
      deployment1 = "${env.branchname == 'release-r2' ? 'sme-sr-workers-r2' : 'sme-sr-workers' }"
      namespace = "${env.branchname == 'pre-prod' ? 'sme-relatorios-d1' : env.branchname == 'development' ? 'relatorios-dev' : env.branchname == 'release' ? 'relatorios-hom' : env.branchname == 'release-r2' ? 'relatorios-hom2' : 'sme-relatorios' }"
    }
  
    agent none

    options {
      buildDiscarder(logRotator(numToKeepStr: '20', artifactNumToKeepStr: '5'))
      disableConcurrentBuilds()
      skipDefaultCheckout()
    }
  
    stages {

        stage('Build') {
	      agent { kubernetes { 
                  label 'builder'
                  defaultContainer 'builder'
                }
              }
          when { anyOf { branch 'master'; branch 'main'; branch 'pre-prod'; branch "story/*"; branch 'development'; branch 'release'; branch 'release-r2'; } } 
          steps {
	    checkout scm
            script {
              imagename1 = "registry.sme.prefeitura.sp.gov.br/${env.branchname}/sme-sr-worker"   
              dockerImage1 = docker.build(imagename1, "-f src/SME.SR.Workers.SGP/Dockerfile .")
              docker.withRegistry( 'https://registry.sme.prefeitura.sp.gov.br', registryCredential ) {
                dockerImage1.push()
              }
              sh "docker rmi $imagename1"
            }
          }
        }
	    
        stage('Deploy'){
	      agent { kubernetes { 
                  label 'builder'
                  defaultContainer 'builder'
                }
              }
            when { anyOf {  branch 'master'; branch 'main' ; branch 'pre-prod'; branch 'development'; branch 'homolog'; branch 'release'; branch 'release-r2'; } }        
            steps {
                script{
                    if ( env.branchname == 'main' ||  env.branchname == 'master' ||  env.branchname == 'pre-prod' ) {
                         withCredentials([string(credentialsId: 'aprovadores-sgp', variable: 'aprovadores')]) {
                            timeout(time: 24, unit: "HOURS") {
                                input message: 'Deseja realizar o deploy?', ok: 'SIM', submitter: "${aprovadores}"
                            }
                        }
                    }
					
                          withCredentials([file(credentialsId: "${kubeconfig}", variable: 'config')]){
			    sh('rm -f '+"$home"+'/.kube/config')
                            sh('cp $config '+"$home"+'/.kube/config')
                            sh "kubectl rollout restart deployment/${deployment1} -n ${namespace}"
                            sh('rm -f '+"$home"+'/.kube/config')
                          }
                }
            }           
        }    
    }
}
def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")
    withCredentials([string(credentialsId: 'telegramToken', variable: 'TOKEN'),
    string(credentialsId: 'telegramChatId', variable: 'CHAT_ID')]) {
        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: 'https://api.telegram.org/bot'+"$TOKEN"+'/sendMessage?text='+encodedMessage+'&chat_id='+"$CHAT_ID"+'&disable_web_page_preview=true',
                validResponseCodes: '200')
        return response
    }
}
def getKubeconf(branchName) {
    if("main".equals(branchName)) { return "config_prd"; }
    else if ("master".equals(branchName)) { return "config_prd"; }
    else if ("pre-prod".equals(branchName)) { return "config_prd"; }
    else if ("homolog".equals(branchName)) { return "config_release"; }
    else if ("release".equals(branchName)) { return "config_release"; }
    else if ("release-r2".equals(branchName)) { return "config_release"; }
    else if ("development".equals(branchName)) { return "config_release"; }
    else if ("develop".equals(branchName)) { return "config_release"; }
}
