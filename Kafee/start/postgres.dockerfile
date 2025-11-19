FROM postgres:latest

ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres
ENV POSTGRES_DB=pre-aspire

# docker build -f postgres.dockerfile -t pre-aspire .
# docker run -d --name pre-aspire-container -p 5432:5432 pre-aspire 