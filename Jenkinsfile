pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-u root'
        }
    }

    options {
        skipStagesAfterUnstable()
    }

    stages {
        stage('Build') {
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build --no-restore --collect "XPlat Code Coverage"'
            }
        }

        stage('Deliver') {
            steps {
                sh 'rm -rf published'
                sh 'dotnet publish "Project 1.csproj" --no-restore -o published'
            }
            post {
                success {
                    archiveArtifacts artifacts: 'published/**'
                }
            }
        }
        stage('Image'){
            steps{
                sh 'curl -fsSL https://get.docker.com -o get-docker.sh'
                sh './get-docker.sh --dry-run'
                sh 'docker build -t auction .'
            }
        }

    }
}
