﻿using System;

namespace CleanDeal.Services.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string html, CancellationToken ct = default);
}
