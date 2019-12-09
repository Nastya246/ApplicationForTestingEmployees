CREATE TABLE [dbo].[Вопросы] (
    [id_вопроса]    INT        IDENTITY (1, 1) NOT NULL,
    [id_Теста]      INT        NOT NULL,
    [Текст вопроса] NVARCHAR(MAX) NULL,
    [Тип ответа]    NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Вопросы] PRIMARY KEY CLUSTERED ([id_вопроса] ASC),
    CONSTRAINT [FK_Вопросы_Тесты1] FOREIGN KEY ([id_Теста]) REFERENCES [dbo].[Тесты] ([id_теста])
);

