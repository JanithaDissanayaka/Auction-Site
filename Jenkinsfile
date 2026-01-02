pipeline {
    agent any

    stages {
        stage('restore'){
            steps{

                sh 'echo "restore"'
                sh 'dotnet restore'
                
            }
        }

        stage('Build') { 
            steps {
                 
                sh 'echo "build"'
                sh 'dotnet build --no-restore' 
            }
        }
        stage('test'){
            steps{
                sh 'echo "test"'
                sh 'dotnet test'
            }
        }
    }
}
