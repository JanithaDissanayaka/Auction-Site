pipeline {
    agent any

    options {
        skipStagesAfterUnstable()
    }

    stages {

        stage('Build & Test') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:8.0'
                    args '-u root'
                }
            }
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
                sh 'dotnet test --no-build --no-restore'
                sh 'rm -rf published'
                sh 'dotnet publish "Project 1.csproj" --no-restore -o published'
            }
            post {
                success {
                    archiveArtifacts artifacts: 'published/**'
                }
            }
        }

        stage('Image') {
            agent {
                docker {
                    image 'docker:cli'
                    args '-u root -v /var/run/docker.sock:/var/run/docker.sock'
                }
            }
            steps {
                sh 'docker version'
                sh 'docker build -t auction .'
            }
        }
    }
}
