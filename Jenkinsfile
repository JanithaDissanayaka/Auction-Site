pipeline {
    agent any

    stages {
        stage('Build app') { 
            steps {
                echo "Building the application..."
                sh 'dotnet restore' 
                sh 'dotnet build --no-restore' 
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    echo "Building the Docker image..."
                    sh 'docker build -t auction:${BUILD_NUMBER} .'
                }
            }
        }
    }

    post {
        success {
            echo "✅ Pipeline completed successfully"
        }
        failure {
            echo "❌ Pipeline failed"
        }
    }
}
