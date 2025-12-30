pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock'
        }
    }

    stages {
        stage('Build app') {
            steps {
                echo "Building the application..."
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo "Building the Docker image..."
                sh 'docker build -t auction:${BUILD_NUMBER} .'
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
