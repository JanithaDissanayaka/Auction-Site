/*pipeline {
    agent any

    stages {
        stage('restore'){
            steps{
                sh 'dotnet restore'
            }
        }

        stage('Build') { 
            steps {
                 
                sh 'dotnet build --no-restore' 
            }
        }
        stage('test'){
            steps{
                sh 'dotnet test'
            }
        }
    }
}
