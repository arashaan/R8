using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;

using MimeKit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.Social
{
    public class SecretCredentials
    {
        public Email Email { get; set; }
        public Whatsapp Whatsapp { get; set; }
        public Facebook Facebook { get; set; }
        public Google Google { get; set; }
        public ReCaptcha ReCaptcha { get; set; }
    }

    public class ReCaptcha
    {
        public string SecretKey { get; set; }
        public string SiteKey { get; set; }
        public string ContentSecurityPolicy { get; set; }
    }

    public class Google
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Facebook
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }

    [Obsolete]
    public class Whatsapp
    {
        public string AccessToken { get; set; }
        public string Sender { get; set; }
        //private Balance _balance;

        //public override string ToString()
        //{
        //    return Sender;
        //}

        //private Client CreateClient()
        //{
        //    var features = new List<Client.Features> { Client.Features.EnableWhatsAppSandboxConversations };

        //    var client = Client.CreateDefault(AccessToken, null, features.ToArray());

        //    _balance = client.Balance();
        //    if (_balance.Amount == "0")
        //        return null;

        //    return client;
        //}

        //public void CreateContact(long receiptNumber)
        //{
        //    var client = CreateClient();
        //    if (client == null)
        //        return;

        //    var contact = client.CreateContact(receiptNumber);
        //    if (contact == null)
        //    {
        //    }
        //}
    }

    public class Email
    {
        public string Server { get; set; }
        public string DefaultSenderName { get; set; }
        public int OutgoingPort { get; set; }
        public int IncomingPort { get; set; }
        public string DomainName { get; set; }

        private async Task<bool> AuthenticateAsync(IMailService client, int port, string username, string password, CancellationToken cancellationToken = default)
        {
            await client.ConnectAsync(Server, port, true, cancellationToken);
            if (!client.IsConnected)
                return false;

            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(username, password, cancellationToken);
            return client.IsAuthenticated;
        }

        public async Task<bool> PingAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            bool result;
            using var client = new SmtpClient();
            try
            {
                result = await AuthenticateAsync(client, OutgoingPort, username, password, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = false;
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);
                client.Dispose();
            }

            return result;
        }

        public async Task<List<IMessageSummary>> ReceiveAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var result = new List<IMessageSummary>();
            using var client = new ImapClient();
            try
            {
                var authenticated = await AuthenticateAsync(client, IncomingPort, username, password, cancellationToken: cancellationToken);
                if (authenticated)
                {
                    var inbox = client.Inbox;
                    //var inbox = client.GetFolderAsync(MailKit.SpecialFolder.Trash);

                    await inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken).ConfigureAwait(false);
                    var uniqueIds = await inbox.SearchAsync(SearchQuery.All, cancellationToken).ConfigureAwait(false);
                    if (uniqueIds?.Any() == true)
                    {
                        var summaries = await inbox.FetchAsync(uniqueIds, MessageSummaryItems.All, cancellationToken);
                        result.AddRange(summaries);
                    }

                    await inbox.CloseAsync(cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);
                client.Dispose();
            }

            return result;
        }

        public async Task<MimeMessage> ReadMessageAsync(string username, string password, UniqueId uniqueId, CancellationToken cancellationToken = default)
        {
            MimeMessage message;
            using var client = new ImapClient();
            try
            {
                var authenticated = await AuthenticateAsync(client, IncomingPort, username, password, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (authenticated)
                {
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken).ConfigureAwait(false);
                    message = await inbox.GetMessageAsync(uniqueId, cancellationToken).ConfigureAwait(false);
                    await inbox.CloseAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    message = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                message = null;
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
                client.Dispose();
            }

            return message;
        }

        public async Task<bool> SendAsync(string username, string password, MimeMessage mailMessage, string subject = null, CancellationToken cancellationToken = default)
        {
            var senderName = DefaultSenderName;
            if (!string.IsNullOrEmpty(subject))
                senderName += $" - {subject}";

            var sender = new MailboxAddress(Encoding.UTF8, senderName, username);
            mailMessage.From.Add(sender);

            using var client = new SmtpClient();
            try
            {
                var authenticated = await AuthenticateAsync(client, OutgoingPort, username, password, cancellationToken);
                if (!authenticated)
                    return false;

                await client.SendAsync(mailMessage, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);
                client.Dispose();
            }
        }
    }
}