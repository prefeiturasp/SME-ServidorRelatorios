pipeline {
  agent {
    node { 
      label 'dockerdotnet2'
    }
  }

  options {
    buildDiscarder(logRotator(numToKeepStr: '5', artifactNumToKeepStr: '5'))
    disableConcurrentBuilds()
    skipDefaultCheckout()
  }

  stages {
    stage('CheckOut') {
      steps {
        checkout scm  
      }
    }
        
    stage('Docker build DEV') {
      when {
        branch 'development'
      }
      steps {
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "42907c0b-0fec-4490-9feb-8ad4f49c810a",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }

    stage('Deploy DEV') {
      when {
        branch 'development'
      }
      steps {
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "8aab17fb-1608-4b53-aff8-d8fef2ff7907",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }
		
	  stage('Docker build HOM') {
      when {
          branch 'release'
      }
      steps {
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "f02e7732-b24d-4c57-97fd-ffe760aa35c3",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }
       
    stage('Deploy HOM') {
      when {
        branch 'release'
      }
      steps {
        timeout(time: 24, unit: "HOURS") {
            telegramSend("${JOB_NAME}...O Build ${BUILD_DISPLAY_NAME} - Requer uma aprovação para deploy !!!\n Consulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)\n")
            input message: 'Deseja realizar o deploy?', ok: 'SIM', submitter: 'marlon_goncalves, robson_silva, marcos_costa, bruno_alevato, rafael_losi'
        }
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "d8034b93-d264-44cc-abc4-0ef25ae670e4",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }

    stage('Docker build HOM-r2') {
      when {
        branch 'release-r2'
      }
      steps {
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "be58097a-881e-4d6e-a961-9d53102a6dfd",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }
       
    stage('Deploy HOM-r2') {
      when {
        branch 'release-r2'
      }
      steps {
        timeout(time: 24, unit: "HOURS") {
            telegramSend("${JOB_NAME}...O Build ${BUILD_DISPLAY_NAME} - Requer uma aprovação para deploy !!!\n Consulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)\n")
            input message: 'Deseja realizar o deploy?', ok: 'SIM', submitter: 'marlon_goncalves, allan_santos, everton_nogueira, marcos_costa, bruno_alevato, rafael_losi'
        }
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "1be62eca-c4a7-4ea5-a1f1-5b2b83661020",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }

	  stage('Docker build PROD') {
      when {
        branch 'master'
      }
      steps {
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "856ef8c0-98fa-4c4d-82ad-4be33347414f",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }
    
    stage('Deploy PROD') {
      when {
        branch 'master'
      }
      steps {
        timeout(time: 24, unit: "HOURS") {
          telegramSend("${JOB_NAME}...O Build ${BUILD_DISPLAY_NAME} - Requer uma aprovação para deploy !!!\n Consulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)\n")
          input message: 'Deseja realizar o deploy?', ok: 'SIM', submitter: 'marlon_goncalves, allan_santos, everton_nogueira, marcos_costa, bruno_alevato, rafael_losi'
        }
        script {
          step([$class: "RundeckNotifier",
          includeRundeckLogs: true,
          jobId: "000eac14-7727-4b6a-aeb8-c4ed71e9340b",
          nodeFilters: "",
          rundeckInstance: "Rundeck-SME",
          shouldFailTheBuild: true,
          shouldWaitForRundeckJob: true,
          tags: "",
          tailLog: true])
        }
      }
    }
  }

  post {
    always {
      echo 'One way or another, I have finished'
    }
    success {
      telegramSend("${JOB_NAME}...O Build ${BUILD_DISPLAY_NAME} - Esta ok !!!\n Consulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)\n\n Uma nova versão da aplicação esta disponivel!!!")
    }
    unstable {
      telegramSend("O Build ${BUILD_DISPLAY_NAME} <${env.BUILD_URL}> - Esta instavel ...\nConsulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)")
    }
    failure {
      telegramSend("${JOB_NAME}...O Build ${BUILD_DISPLAY_NAME}  - Quebrou. \nConsulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)")
    }
    changed {
      echo 'Things were different before...'
    }
    aborted {
      telegramSend("O Build ${BUILD_DISPLAY_NAME} - Foi abortado.\nConsulte o log para detalhes -> [Job logs](${env.BUILD_URL}console)")
    }
  }
}
