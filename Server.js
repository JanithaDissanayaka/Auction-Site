// Import dependencies
require('dotenv').config();
const express = require('express');
const sendGridMail = require('@sendgrid/mail');
const twilio = require('twilio');

// Initialize Express app
const app = express();
app.use(express.json()); // Middleware to parse JSON bodies

// Set up SendGrid and Authy API keys
sendGridMail.setApiKey(process.env.SENDGRID_API_KEY);
const authyClient = new twilio.Authy(process.env.AUTHY_API_KEY);

// Helper function to send email
function sendEmail(to, subject, content) {
    const message = {
        to,
        from: 'your_email@example.com', // Replace with your verified SendGrid sender email
        subject,
        text: content,
    };

    return sendGridMail.send(message)
        .then(() => console.log("Email sent successfully"))
        .catch((error) => console.error("Error sending email:", error));
}

// Helper function to request 2FA code
function request2FACode(userPhoneNumber, countryCode = '1') {
    return authyClient.startPhoneVerification({
        country_code: countryCode,
        phone_number: userPhoneNumber,
        via: 'sms'
    }).then(response => console.log("2FA code sent"))
        .catch(error => console.error("Error sending 2FA code:", error));
}

// Helper function to verify 2FA code
function verify2FACode(userPhoneNumber, countryCode = '1', code) {
    return authyClient.verifyPhone({
        country_code: countryCode,
        phone_number: userPhoneNumber,
        verification_code: code
    }).then(response => console.log("2FA code verified"))
        .catch(error => console.error("Error verifying 2FA code:", error));
}

// API endpoint to send an email
app.post('/send-email', (req, res) => {
    const { email, subject, message } = req.body;
    sendEmail(email, subject, message)
        .then(() => res.status(200).send("Email sent"))
        .catch(() => res.status(500).send("Failed to send email"));
});

// API endpoint to request a 2FA code
app.post('/request-2fa', (req, res) => {
    const { phone } = req.body;
    request2FACode(phone)
        .then(() => res.status(200).send("2FA code sent"))
        .catch(() => res.status(500).send("Failed to send 2FA code"));
});

// API endpoint to verify a 2FA code
app.post('/verify-2fa', (req, res) => {
    const { phone, code } = req.body;
    verify2FACode(phone, code)
        .then(() => res.status(200).send("2FA verified"))
        .catch(() => res.status(401).send("Invalid 2FA code"));
});

// Start the server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
