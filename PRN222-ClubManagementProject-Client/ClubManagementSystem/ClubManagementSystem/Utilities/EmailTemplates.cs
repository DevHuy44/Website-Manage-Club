namespace ClubManagementSystem.Utilities
{
    public class EmailTemplates
    {
        public static string GetRemindFeesTemplate(string fullName)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f3f4f6;
                    margin: 0;
                    padding: 0;
                    width: 100%;
                }}
                table {{
                    width: 100%;
                    height: 100vh;
                    border-collapse: collapse;
                }}
                .container {{
                    max-width: 480px;
                    background: #ffffff;
                    padding: 24px;
                    border-radius: 12px;
                    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
                    text-align: center;
                }}
                .logo {{
                    width: 120px;
                    margin-bottom: 16px;
                }}
                .title {{
                    font-size: 22px;
                    font-weight: bold;
                    margin-bottom: 12px;
                    color: #333;
                }}
                .text {{
                    font-size: 16px;
                    color: #555;
                    margin-bottom: 20px;
                }}
                .button {{
                    display: inline-block;
                    padding: 12px 24px;
                    color: #ffffff;
                    background: linear-gradient(135deg, #6EC6FF, #42A5F5);
                    text-decoration: none;
                    border-radius: 25px;
                    font-size: 16px;
                    font-weight: bold;
                    transition: 0.3s;
                    box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
                }}
                .button:hover {{
                    background: linear-gradient(135deg, #90CAF9, #64B5F6);
                    transform: translateY(-2px);
                    box-shadow: 0 5px 12px rgba(0, 0, 0, 0.2);
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #777;
                }}
                .img {{
                    width: 70%;
                    height: 70%;
                }}
            </style>
        </head>
        <body>
            <table>
                <tr>
                    <td align='center'>
                        <div class='container'>
                            <img class='logo' src='https://media.istockphoto.com/id/1412758509/photo/calendar-with-clock-and-notification-bell.jpg?s=612x612&w=0&k=20&c=vHCEX1MoWg4W5XzwYXXGxzOVDbq9GAffoH4ulgK-4dA=' alt='Company Logo' />
                            <h2 class='title'>Yêu Cầu Đóng Phí Trước Thời Hạn</h2>
                            <p class='text'>Xin chào <strong>{fullName}</strong>,</p>
                            <p class='text'>Chúng tôi nhắc nhỡ bạn về việc nộp phí cho câu lạc bộ</p>             
                            
                            <div class='footer'>
                                <p>Trân trọng,</p>
                                <p><strong>CLB của bạn</strong></p>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>
    ";
        }
    

    public static string GetPaymentSuccessTemplate(string fullName)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f3f4f6;
                    margin: 0;
                    padding: 0;
                    width: 100%;
                }}
                table {{
                    width: 100%;
                    height: 100vh;
                    border-collapse: collapse;
                }}
                .container {{
                    max-width: 480px;
                    background: #ffffff;
                    padding: 24px;
                    border-radius: 12px;
                    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
                    text-align: center;
                }}
                .logo {{
                    width: 120px;
                    margin-bottom: 16px;
                }}
                .title {{
                    font-size: 22px;
                    font-weight: bold;
                    margin-bottom: 12px;
                    color: #333;
                }}
                .text {{
                    font-size: 16px;
                    color: #555;
                    margin-bottom: 20px;
                }}
                .button {{
                    display: inline-block;
                    padding: 12px 24px;
                    color: #ffffff;
                    background: linear-gradient(135deg, #6EC6FF, #42A5F5);
                    text-decoration: none;
                    border-radius: 25px;
                    font-size: 16px;
                    font-weight: bold;
                    transition: 0.3s;
                    box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
                }}
                .button:hover {{
                    background: linear-gradient(135deg, #90CAF9, #64B5F6);
                    transform: translateY(-2px);
                    box-shadow: 0 5px 12px rgba(0, 0, 0, 0.2);
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #777;
                }}
                .img {{
                    width: 70%;
                    height: 70%;
                }}
            </style>
        </head>
        <body>
            <table>
                <tr>
                    <td align='center'>
                        <div class='container'>
                            <img class='logo' src='https://static.vecteezy.com/system/resources/previews/015/876/264/non_2x/success-payment-in-hand-illustration-in-flat-style-approved-money-illustration-on-isolated-background-successful-pay-sign-business-concept-vector.jpg' alt='Company Logo' />
                            <h2 class='title'>Thanh toán phí CLB</h2>
                            <p class='text'>Xin chào <strong>{fullName}</strong>,</p>
                            <p class='text'>Bạn đã thanh toán phí thành công</p>             
                            
                            <div class='footer'>
                                <p>Trân trọng,</p>
                                <p><strong>CLB của bạn</strong></p>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>
    ";
        }

        public static string GetRemindExpiredTemplate(string fullName)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f3f4f6;
                    margin: 0;
                    padding: 0;
                    width: 100%;
                }}
                table {{
                    width: 100%;
                    height: 100vh;
                    border-collapse: collapse;
                }}
                .container {{
                    max-width: 480px;
                    background: #ffffff;
                    padding: 24px;
                    border-radius: 12px;
                    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
                    text-align: center;
                }}
                .logo {{
                    width: 120px;
                    margin-bottom: 16px;
                }}
                .title {{
                    font-size: 22px;
                    font-weight: bold;
                    margin-bottom: 12px;
                    color: #333;
                }}
                .text {{
                    font-size: 16px;
                    color: #555;
                    margin-bottom: 20px;
                }}
                .button {{
                    display: inline-block;
                    padding: 12px 24px;
                    color: #ffffff;
                    background: linear-gradient(135deg, #6EC6FF, #42A5F5);
                    text-decoration: none;
                    border-radius: 25px;
                    font-size: 16px;
                    font-weight: bold;
                    transition: 0.3s;
                    box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
                }}
                .button:hover {{
                    background: linear-gradient(135deg, #90CAF9, #64B5F6);
                    transform: translateY(-2px);
                    box-shadow: 0 5px 12px rgba(0, 0, 0, 0.2);
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #777;
                }}
                .img {{
                    width: 70%;
                    height: 70%;
                }}
            </style>
        </head>
        <body>
            <table>
                <tr>
                    <td align='center'>
                        <div class='container'>
                            <img class='logo' src='https://png.pngtree.com/png-vector/20220527/ourmid/pngtree-red-grunge-expired-rubber-stamp-png-image_4750216.png' alt='Company Logo' />
                            <h2 class='title'>Hết hạn nộp phí CLB</h2>
                            <p class='text'>Xin chào <strong>{fullName}</strong>,</p>
                            <p class='text'>Bạn đã hết hạn nộp phí cho CLB</p>             
                            
                            <div class='footer'>
                                <p>Trân trọng,</p>
                                <p><strong>CLB của bạn</strong></p>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>
    ";
        }
    }
}
